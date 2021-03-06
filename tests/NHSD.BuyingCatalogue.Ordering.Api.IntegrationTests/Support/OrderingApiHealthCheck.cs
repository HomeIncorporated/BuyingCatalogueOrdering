﻿using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    [Binding]
    public sealed class OrderingApiHealthCheck
    {
        internal async Task AwaitApiRunningAsync(Settings settings)
        {
            var _baseUrl = settings.OrderingApiBaseUrl;
            TimeSpan testTimeOut = TimeSpan.FromSeconds(settings.OrderingApiHealthCheckTimeout);

            await AwaitApiRunningAsync(($"{_baseUrl}/health/live"), testTimeOut);
            await AwaitApiRunningAsync(($"{_baseUrl}/health/ready"), testTimeOut);
        }

        internal async Task AwaitApiRunningAsync(string url, TimeSpan testTimeOut)
        {
            var started = await HttpClientAwaiter.WaitForGetAsync(url, testTimeOut);
            if (!started)
            {
                throw new Exception($"Start Ordering API failed, could not get a successful health status from '{url}' after trying for '{testTimeOut}'");
            }
        }
    }
}
