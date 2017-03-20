using Newtonsoft.Json;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Html;
using Orchard.Services;
using Orchard.Settings;
using Orchard.UI.Notify;
using OShop.Models;
using OShop.PayPal.Models;
using OShop.PayPal.Models.Api;
using OShop.PayPal.Services;
using OShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OShop.PayPal.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IPaypalApiService _apiService;
        private readonly IClock _clock;
        private readonly ISiteService _siteService;

        public PaymentController(
            IPaymentService paymentService,
            ICurrencyProvider currencyProvider,
            IPaypalApiService apiService,
            IClock clock,
            ISiteService siteService,
            IOrchardServices services) {
            _paymentService = paymentService;
            _currencyProvider = currencyProvider;
            _apiService = apiService;
            _clock = clock;
            _siteService = siteService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        /// <summary>
        /// Create PayPal payment for a ContentItem with a PaymentPart
        /// </summary>
        /// <param name="Id">ContentItem Id</param>
        public async Task<ActionResult> Create(int Id)
        {
            var paymentPart = Services.ContentManager.Get<PaymentPart>(Id);
            if (paymentPart == null) {
                return new HttpNotFoundResult();
            }

            decimal OutstandingAmount = paymentPart.PayableAmount - paymentPart.AmountPaid;
            if (OutstandingAmount <= 0) {
                Services.Notifier.Information(T("Nothing left to pay on this document."));
                return Redirect(Url.ItemDisplayUrl(paymentPart));
            }

            var transaction = new PaymentTransactionRecord() {
                Method = "PayPal",
                Amount = OutstandingAmount,
                Date = _clock.UtcNow,
                Status = TransactionStatus.Pending
            };

            _paymentService.AddTransaction(paymentPart, transaction);

            var siteSettings = _siteService.GetSiteSettings();
            string baseUrl = siteSettings.BaseUrl.TrimEnd(new char[] {' ', '/'});

            try {
                // Create web profile
                string webProfileId = await _apiService.CreateWebProfile(new WebProfile() {
                    // Ensure unique name
                    Name = paymentPart.Reference + "_" + transaction.Id,
                    InputFields = new InputFields() { allow_note = false, NoShipping = 1 },
                    Presentation = new Presentation() { BrandName = siteSettings.SiteName }
                });
                // Create payment
                var paymentCtx = await _apiService.CreatePaymentAsync(new Payment() {
                    Intent = PaymentIntent.Sale,
                    Payer = new Payer() { PaymentMethod = PaymentMethod.Paypal },
                    Transactions = new List<Transaction>() {
                        new Transaction() {
                            Amount = new Amount(){
                                Total = OutstandingAmount,
                                Currency = _currencyProvider.IsoCode
                            },
                            Description = paymentPart.Reference + " - " + OutstandingAmount.ToString("C", _currencyProvider.NumberFormat)
                        }
                    },
                    RedirectUrls = new RedirectUrls() {
                        ReturnUrl = baseUrl + Url.Action("Execute", new { id = transaction.Id }),
                        CancelUrl = baseUrl + Url.Action("Cancel", new { id = transaction.Id })
                    },
                    ExperienceProfileId = webProfileId
                });

                if (paymentCtx != null && paymentCtx.Payment.State == PaymentState.Created) {
                    string approvalUrl = paymentCtx.Payment.Links.Where(lnk => lnk.Relation == "approval_url").Select(lnk => lnk.Href).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(approvalUrl)) {
                        throw new Exception("Missing approval_url from Paypal response.");
                    }
                    transaction.TransactionId = paymentCtx.Payment.Id;
                    transaction.Data = JsonConvert.SerializeObject(paymentCtx);
                    if (paymentCtx.UseSandbox) {
                        transaction.Method += " #SANDBOX#";
                    }
                    _paymentService.UpdateTransaction(transaction);
                    return Redirect(approvalUrl);
                }
                else {
                    Services.Notifier.Error(T("Payment creation failed."));
                }
            }
            catch {
                Services.Notifier.Error(T("We are encountering issues with PayPal service."));
            }
            return Redirect(Url.ItemDisplayUrl(paymentPart));
        }

        /// <summary>
        /// Execute an approved PayPal payment
        /// </summary>
        /// <param name="Id">PaymentTransactionRecord Id</param>
        public async Task<ActionResult> Execute(int Id, string PaymentId, string PayerId) {
            var transactionRecord = _paymentService.GetTransaction(Id);
            if (transactionRecord == null) {
                return new HttpNotFoundResult();
            }

            var paymentCtx = JsonConvert.DeserializeObject<PaymentContext>(transactionRecord.Data);
            if(paymentCtx != null) {
                Transaction paypalTransaction = null;
                try {
                    paymentCtx = await _apiService.ExecutePaymentAsync(paymentCtx, PayerId);
                    paypalTransaction = paymentCtx.Payment.Transactions.Single();
                }
                catch {
                    Services.Notifier.Error(T("We are encountering issues with PayPal service."));
                }
                if (paypalTransaction != null && paymentCtx.Payment.State == PaymentState.Approved) {
                    transactionRecord.Data = JsonConvert.SerializeObject(paymentCtx);
                    transactionRecord.Date = _clock.UtcNow;
                    transactionRecord.Status = TransactionStatus.Validated;
                    transactionRecord.Amount = paymentCtx.Payment.Transactions.Select(t => t.Amount).Where(a => a.Currency == _currencyProvider.IsoCode).Sum(a => a.Total) ;
                    _paymentService.UpdateTransaction(transactionRecord);
                    Services.Notifier.Information(T("Your payment was successfully registered."));
                }
                else {
                    Services.Notifier.Error(T("We were unable to execute your payment."));
                }
            }
            else {
                Services.Notifier.Error(T("We were unable to execute your payment."));
            }

            var content = Services.ContentManager.Get(transactionRecord.PaymentPartRecord.ContentItemRecord.Id);
            return Redirect(Url.ItemDisplayUrl(content));
        }

        /// <summary>
        /// Cancel a PayPal payment
        /// </summary>
        /// <param name="Id">PaymentTransactionRecord Id</param>
        public ActionResult Cancel(int Id) {
            var transactionRecord = _paymentService.GetTransaction(Id);

            if (transactionRecord != null) {
                transactionRecord.Status = TransactionStatus.Canceled;
                transactionRecord.Date = _clock.UtcNow;
                _paymentService.UpdateTransaction(transactionRecord);
                Services.Notifier.Information(T("Your payment has been canceled."));

                var content = Services.ContentManager.Get(transactionRecord.PaymentPartRecord.ContentItemRecord.Id);
                if (content != null) {
                    return Redirect(Url.ItemDisplayUrl(content));
                }
                else {
                    return new HttpNotFoundResult();
                }
            }
            else {
                return new HttpNotFoundResult();
            }
        }
    }
}