using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using IdentityModel.Client;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
	[Binding]
	public sealed class OrderSteps
	{
		[When(@"the user executes the get all orders request")]
		public async Task WhenTheUserExecutesTheGetAllOrdersRequest()
		{
			var discoveryAddress = "http://localhost:8070/identity";

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
				return;
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
				return;
			}

			string accessToken = tokenResponse.AccessToken;

			var result = await "http://localhost:8076/api/v1/orders"
				.WithOAuthBearerToken(accessToken)
				.AllowAnyHttpStatus()
				.GetAsync();

			string content = await result.Content.ReadAsStringAsync();

			var success = result.IsSuccessStatusCode;
			Console.WriteLine(success);
		}
	}
}
