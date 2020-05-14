using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Services
{
    public sealed class IdentityTestServer
    {
        private const string OrderingScope = "Ordering";

        public static string Token { get; set; }

        public static IHostBuilder CreateServer()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.ConfigureLogging(builder =>
                    {
                        builder.AddConsole();
                    });

                    webHost.ConfigureAppConfiguration((context, configurationBuilder) =>
                    {
                        configurationBuilder.AddJsonFile("appsettings.json", false)
                            .AddEnvironmentVariables();
                    });

                    webHost.UseUrls("http://localhost:8090");
                    webHost.UseKestrel();
                    webHost.UseStartup<IdentityStartUp>();
                });

            return hostBuilder;
        }

        private sealed class IdentityStartUp
        {
            private readonly IConfiguration _configuration;

            public IdentityStartUp(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddIdentityServer(options =>
                    {
                        options.IssuerUri = "http://localhost:8090/identity";
                    })
                    .AddInMemoryClients(Clients.Get())
                    .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                    .AddInMemoryApiResources(Resources.GetApiResources())
                    .AddTestUsers(Users.Get())
                    .AddDeveloperSigningCredential();
            }

            public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
            {
                var logger = loggerFactory.CreateLogger<Startup>();

                app.Use(async (context, next) =>
                {
                    logger.LogError($"***Request*** : {context.Request.Path}");

                    await next();
                });

                app.Map("/identity", builder =>
                {
                    builder.UseIdentityServer();
                });
            }
        }

        private static class Clients
        {
            public static IEnumerable<Client> Get()
            {
                return new List<Client>
                {
                    new Client
                    {
                        ClientId = "PasswordClient",
                        ClientName = "Password Client",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        AccessTokenLifetime = 3600,
                        AllowOfflineAccess = false,
                        ClientSecrets = new List<Secret> {new Secret("PasswordSecret".Sha256())},
                        AllowedScopes = new List<string>
                        {
                            "openid",
                            "email",
                            "profile",
                            "Organisation",
                            OrderingScope
                        }
                    }
                };
            }
        }

        private static class Resources
        {
            public static IEnumerable<IdentityResource> GetIdentityResources()
            {
                var profileResource = new IdentityResources.Profile();
                profileResource.UserClaims.Add("PrimaryOrganisationId");
                profileResource.UserClaims.Add("OrganisationFunction");

                return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(), profileResource, new IdentityResources.Email()
                };
            }

            public static IEnumerable<ApiResource> GetApiResources()
            {
                return new List<ApiResource>
                {
                    new ApiResource
                    {
                        Name = OrderingScope,
                        DisplayName = OrderingScope,
                        UserClaims = new List<string> {"ordering"},
                        Scopes = new List<Scope>
                        {
                            new Scope(OrderingScope)
                            {
                                UserClaims = new List<string>
                                {
                                    "sub",
                                    "client_id",
                                    "preferred_username",
                                    "unique_name",
                                    "given_name",
                                    "family_name",
                                    "name",
                                    "email",
                                    "email_verified",
                                    "primaryOrganisationId",
                                    "organisationFunction",
                                    "organisation",
                                    "account",
                                    "ordering"
                                }
                            }
                        }
                    }
                };
            }
        }

        private static class Users
        {
            public static List<TestUser> Get()
            {
                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "80C1CFD3-DED7-437A-BE09-ACD060A25755",
                        Username = "bobsmith@email.com",
                        Password = "Pass123$",
                        Claims = new List<Claim>
                        {
                            new Claim("sub", "7B195137-6A59-4854-B118-62B39A3101EF"),
                            new Claim("client_id", "PasswordClient"),
                            new Claim("preferred_username", "BobSmith@email.com"),
                            new Claim("unique_name", "BobSmith@email.com"),
                            new Claim("given_name", "Bob"),
                            new Claim("family_name", "Smith"),
                            new Claim("name", "Bob Smith"),
                            new Claim("email", "BobSmith@email.com"),
                            new Claim("email_verified", "true"),
                            new Claim("primaryOrganisationId", "c7a94e85-025b-403f-b984-20ee5f9ec333"),
                            new Claim("organisationFunction", "Authority"),
                            new Claim("organisation", "Manage"),
                            new Claim("account", "Manage"),
                            new Claim("ordering", "Manage")
                        }
                    }
                };
            }
        }
    }

}
