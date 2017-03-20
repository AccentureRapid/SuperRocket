using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Services;
using OShop.PayPal.Models;
using OShop.PayPal.Models.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OShop.PayPal.Services {
    public class PaypalApiService : IPaypalApiService {
        private PaypalSettings _settings;

        private PaypalSettings Settings {
            get {
                _settings = _settings ?? _settingsService.GetSettings();
                return _settings;
            }
        }

        private readonly IPaypalSettingsService _settingsService;
        private readonly IPaypalConnectionManager _connectionManager;
        private readonly IClock _clock;

        public PaypalApiService(
            IPaypalSettingsService settingsService,
            IPaypalConnectionManager connectionManager,
            IClock clock) {
            _settingsService = settingsService;
            _connectionManager = connectionManager;
            _clock = clock;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public async Task<PaymentContext> CreatePaymentAsync(Payment Payment) {
            using (var client = await _connectionManager.CreateClientAsync(Settings)) {
                try {
                    var response = await client.PostAsJsonAsync("v1/payments/payment", Payment);
                    if (response.IsSuccessStatusCode) {
                        var createdPayment = await response.Content.ReadAsAsync<Payment>();
                        return new PaymentContext() {
                            UseSandbox = Settings.UseSandbox,
                            Payment = createdPayment
                        };
                    }
                    else {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        Logger.Error("Payment creation failed. ({0}) {1}\r\n{2}", response.StatusCode, response.ReasonPhrase, errorMsg);
                        throw new OrchardException(T("Payment creation failed."));
                    }
                }
                catch(Exception exp) {
                    throw new OrchardException(T("Payment creation failed."), exp);
                }
            }
        }

        public async Task<PaymentContext> ExecutePaymentAsync(PaymentContext PaymentCtx, string PayerId) {
            using (var client = await _connectionManager.CreateClientAsync(Settings)) {
                    try {
                    var response = await client.PostAsJsonAsync("v1/payments/payment/" + PaymentCtx.Payment.Id + "/execute", new { payer_id = PayerId });
                    if (response.IsSuccessStatusCode) {
                        var executedPayment = await response.Content.ReadAsAsync<Payment>();
                        return new PaymentContext() {
                            UseSandbox = PaymentCtx.UseSandbox,
                            Payment = executedPayment
                        };
                    }
                    else {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        Logger.Error("Payment execution failed. ({0}) {1}\r\n{2}", response.StatusCode, response.ReasonPhrase, errorMsg);
                        throw new OrchardException(T("Payment execution failed."));
                    }
                }
                catch (Exception exp) {
                    throw new OrchardException(T("Payment execution failed."), exp);
                }
            }
        }

        public async Task<string> CreateWebProfile(WebProfile Profile) {
            using (var client = await _connectionManager.CreateClientAsync(Settings)) {
                try {
                    var response = await client.PostAsJsonAsync("v1/payment-experience/web-profiles", Profile);
                    if (response.IsSuccessStatusCode) {
                        var result = await response.Content.ReadAsAsync<CreateProfileResponse>();
                        return result.Id;
                    }
                    else {
                        throw new OrchardException(T("Web profile creation failed."));
                    }
                }
                catch (Exception exp) {
                    throw new OrchardException(T("Web profile creation failed."), exp);
                }
            }
        }
    }
}