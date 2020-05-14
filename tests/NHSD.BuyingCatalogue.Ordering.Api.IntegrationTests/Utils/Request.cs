using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using IdentityModel.Client;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal sealed class Request
    {
        private readonly Response _response;
        private readonly ScenarioContext _context;

        public Request(Response response, ScenarioContext context)
        {
            _response = response;
            _context = context;
        }

        public async Task GetAsync(string url, params object[] pathSegments)
        {
            string token = await GetAccessTokenAsync();
            _response.Result = await CreateCommonRequest(url, token, pathSegments).GetAsync();
        }

        public async Task PostJsonAsync(string url, object payload, params object[] pathSegments)
        {
            string token = await GetAccessTokenAsync();
            _response.Result = await CreateCommonRequest(url, token, pathSegments).PostJsonAsync(payload);
        }

        public async Task PutJsonAsync(string url, object payload, params object[] pathSegments)
        {
            string token = await GetAccessTokenAsync();
            _response.Result = await CreateCommonRequest(url, token, pathSegments).PutJsonAsync(payload);
        }

        private IFlurlRequest CreateCommonRequest(string url, string token, params object[] pathSegments)
        {
            return url
                .AppendPathSegments(pathSegments)
                .WithOAuthBearerToken(_context.Get(ScenarioContextKeys.AccessToken, token))
                .AllowAnyHttpStatus();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var discoveryAddress = "http://host.docker.internal:8070/identity";

            using var client = new HttpClient();

            var discoveryDocument =
                await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Policy = new DiscoveryPolicy { RequireHttps = false },
                    Address = discoveryAddress,
                });

            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return string.Empty;
            }

            TokenResponse tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PasswordClient",
                ClientSecret = "PasswordSecret",
                UserName = "alicesmith@email.com",
                Password = "Pass123$",
                Scope = "Ordering"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return string.Empty;
            }

            return tokenResponse.AccessToken;
        }
    }
}
