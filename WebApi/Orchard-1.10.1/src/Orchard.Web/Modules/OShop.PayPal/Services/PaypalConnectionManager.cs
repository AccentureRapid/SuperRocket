using Orchard;
using Orchard.Caching;
using Orchard.Localization;
using Orchard.Services;
using OShop.PayPal.Models;
using OShop.PayPal.Models.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OShop.PayPal.Services {
    public class PaypalConnectionManager : IPaypalConnectionManager {
        public const string SandboxEndpoint = "https://api.sandbox.paypal.com/";
        public const string LiveEndpoint = "https://api.paypal.com/";

        private readonly ICacheManager _cacheManager;
        private readonly IClock _clock;

        public PaypalConnectionManager(ICacheManager cacheManager, IClock clock) {
            _cacheManager = cacheManager;
            _clock = clock;

            T = NullLocalizer.Instance;

            // Enable TLS v1.2
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
        }

        public Localizer T { get; set; }

        public Task<bool> ValidateCredentialsAsync(string ClientId, string Secret, bool UseSandbox) {
            return Task.Run(() => {
                try {
                    return CreateTokenAsync(ClientId, Secret, UseSandbox).Result != null;
                }
                catch {
                    return false;
                }
            });
        }

        public Task<HttpClient> CreateClientAsync(PaypalSettings Settings) {
            return Task.Run(() => _cacheManager.Get(Settings, true, async ctx => {
                var createdToken = await CreateTokenAsync(Settings);
                if(createdToken != null) {
                    // Acquire new token if it expires in less than 5 minutes.
                    ctx.Monitor(_clock.WhenUtc(_clock.UtcNow.AddSeconds(createdToken.ExpiresIn - 300)));
                }
                return createdToken;
            })).ContinueWith<HttpClient>((getTokenTask) => {
                if (getTokenTask.Result != null) {
                    var token = getTokenTask.Result;
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(Settings.UseSandbox ? PaypalConnectionManager.SandboxEndpoint : PaypalConnectionManager.LiveEndpoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.Token);

                    return client;
                }
                else {
                    throw new OrchardException(T("Unable to obtain Access Token from PayPal API."));
                }
            });
        }

        private Task<AccessToken> CreateTokenAsync(PaypalSettings Settings) {
            return CreateTokenAsync(Settings.ClientId, Settings.ClientSecret, Settings.UseSandbox);
        }

        private Task<AccessToken> CreateTokenAsync(string ClientId, string Secret, bool UseSandbox) {
            var client = new HttpClient();
            client.BaseAddress = new Uri(UseSandbox ? PaypalConnectionManager.SandboxEndpoint : PaypalConnectionManager.LiveEndpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                ASCIIEncoding.ASCII.GetBytes(ClientId + ":" + Secret)
            ));

            return client.PostAsync("v1/oauth2/token", new FormUrlEncodedContent(new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            })).ContinueWith<AccessToken>((createTokenTask) => {
                if(createTokenTask.Result.IsSuccessStatusCode) {
                    return createTokenTask.Result.Content.ReadAsAsync<AccessToken>().Result;
                }
                else {
                    throw new OrchardException(T("Unable to obtain Access Token from PayPal API."));
                }
            });
        }
    }
}