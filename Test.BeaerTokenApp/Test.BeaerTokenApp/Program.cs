using System;

namespace Test.BearerTokenApp
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				TestBearTokenBuilder();
			}
			catch (Exception exception)
			{
				Console.WriteLine("-------------------");
				Console.WriteLine(exception);
				Console.WriteLine("-------------------");
			}
		}

		private static void TestBearTokenBuilder()
		{
			var token = new BearerTokenBuilder()
				.WithSigningCertificate(EmbeddedResourceReader.GetCertificate())
				.ForSubject("7B195137-6A59-4854-B118-62B39A3101EF")
				.WithClaim("client_id", "PasswordClient")
				.WithClaim("preferred_username", "BobSmith@email.com")
				.WithClaim("unique_name", "BobSmith@email.com")
				.WithClaim("given_name", "Bob")
				.WithClaim("family_name", "Smith")
				.WithClaim("name", "Bob Smith")
				.WithClaim("email", "BobSmith@email.com")
				.WithClaim("email_verified", "true")
				.WithClaim("primaryOrganisationId", "c7a94e85-025b-403f-b984-20ee5f9ec333")
				.WithClaim("organisationFunction", "Authority")
				.WithClaim("organisation", "Manage")
				.WithClaim("account", "Manage")
				.BuildToken();

			Console.WriteLine("Token:");
			Console.WriteLine(token);
		}
	}
}
