using Orchard;
using OShop.PayPal.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace OShop.PayPal.Services {
    public interface IPaypalConnectionManager : ISingletonDependency {
        Task<bool> ValidateCredentialsAsync(string ClientId, string Secret, bool UseSandbox);
        Task<HttpClient> CreateClientAsync(PaypalSettings Settings);
    }
}
