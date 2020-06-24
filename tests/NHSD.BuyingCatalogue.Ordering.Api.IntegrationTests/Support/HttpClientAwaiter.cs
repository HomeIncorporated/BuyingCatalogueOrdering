﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    public static class HttpClientAwaiter
    {
        public static async Task<bool> WaitForGetAsync(string address, TimeSpan testTimeout)
        {
            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(1) })
            {
                DateTime startTime = DateTime.UtcNow;
                while (DateTime.UtcNow - startTime < testTimeout)
                {
                    try
                    {
                        using var response = await httpClient.GetAsync(new Uri(address)).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        // Ignore exceptions, just retry
                    }

                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }

            return false;
        }
    }
}
