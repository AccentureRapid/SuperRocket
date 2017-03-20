using Orchard;
using OShop.PayPal.Models;
using OShop.PayPal.Models.Api;
using System.Threading.Tasks;

namespace OShop.PayPal.Services {
    public interface IPaypalApiService : IDependency {
        Task<PaymentContext> CreatePaymentAsync(Payment Payment);
        Task<PaymentContext> ExecutePaymentAsync(PaymentContext PaymentCtx, string PayerId);
        Task<string> CreateWebProfile(WebProfile Profile);
    }
}
