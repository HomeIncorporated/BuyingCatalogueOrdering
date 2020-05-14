using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoDi;
using IdentityModel.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Services;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Hooks
{
    [Binding]
    public sealed class IntegrationHook
    {
        private readonly IObjectContainer _objectContainer;
        private IHost _identityHost;

        public IntegrationHook(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
        }

        [BeforeScenario]
        public async Task BeforeScenarioAsync()
        {
            RegisterTestConfiguration();
            RegisterCustomValueRetrievers();

            await StartMockIdentityServerAsync();
            await ResetDatabaseAsync();
        }

        [AfterScenario]
        public async Task AfterScenarioAsync()
        {
            await StopMockIdentityServerAsync();
        }

        private async Task StartMockIdentityServerAsync()
        {
            _identityHost = await IdentityTestServer.CreateServer().StartAsync();

            // Create an HttpClient to send requests to the TestServer
            //var client = _identityHost.GetTestClient();

            using HttpClient client = new HttpClient();

            //var discoveryAddress = client.BaseAddress.ToString();
            var discoveryAddress = "http://localhost:5102/identity";

            var discoveryDocument =
                await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Policy = new DiscoveryPolicy { RequireHttps = false },
                    Address = discoveryAddress,
                });

            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return;
            }

            TokenResponse tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PasswordClient",
                ClientSecret = "PasswordSecret",
                UserName = "bobsmith@email.com",
                Password = "Pass123$",
                Scope = "openid profile email Ordering"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            IdentityTestServer.Token = tokenResponse.AccessToken;
        }

        private async Task StopMockIdentityServerAsync()
        {
            var identityHost = _identityHost;
            if (identityHost is object)
            {
                await identityHost.StopAsync();
                _identityHost = null;
            }
        }

        public void RegisterTestConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            _objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
        }

        private static void RegisterCustomValueRetrievers()
        {
            var valueRetrievers = Service.Instance.ValueRetrievers;

            valueRetrievers.Register(new DateTimeValueRetriever());
        }

        private async Task ResetDatabaseAsync() =>
            await IntegrationDatabase.ResetAsync(_objectContainer.Resolve<IConfiguration>());
    }
}
