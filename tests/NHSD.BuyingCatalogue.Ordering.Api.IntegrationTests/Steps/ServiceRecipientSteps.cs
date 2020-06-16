using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class ServiceRecipientSteps
    {
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly ScenarioContext _context;
        private readonly string _serviceRecipientUrl;

        public ServiceRecipientSteps(Response response, Request request, Settings settings ,ScenarioContext context)
        {
            _response = response;
            _request = request;
            _settings = settings;
            _context = context;
            _serviceRecipientUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/service-recipients";
        }

        [Given(@"Service Recipients exist")]
        public async Task GivenServiceRecipientsExist(Table table)
        {
            foreach (var serviceRecipientItem in table.CreateSet<ServiceRecipientTable>())
            {
                var orderId = _context.GetOrderIdByDescription(serviceRecipientItem.OrderDescription);
                orderId.Should().NotBeNull();
                var serviceRecipient = ServiceRecipientBuilder
                    .Create()
                    .WithOdsCode(serviceRecipientItem.OdsCode)
                    .WithName(serviceRecipientItem.Name)
                    .WithOrderId((int)orderId)
                    .Build();

                await serviceRecipient.InsertAsync(_settings.ConnectionString);
            }
        }

        [When(@"the user makes a request to retrieve the service-recipients section for order with Description (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheService_RecipientsSectionWithOrderDescription(string description)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            await _request.GetAsync(string.Format(_serviceRecipientUrl, orderId));
        }


        [When(@"the user makes a request to retrieve the service-recipients section with unknown orderId")]
        public async Task WhenTheUserMakesARequestToRetrieveTheService_RecipientsSectionWithUnknownOrderID()
        {
            await _request.GetAsync(string.Format(_serviceRecipientUrl, -999));
        }

        [When(@"the user makes a request to set the service-recipients section with order Description (.*)")]
        public async Task WhenTheUserMakesARequestToRetrieveTheService_RecipientsSectionWithOrderID(string description, Table table)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            var payload = new ServiceRecipientsTable { ServiceRecipients = table.CreateSet<ServiceRecipientTable>() };
            await _request.PutJsonAsync(string.Format(_serviceRecipientUrl, orderId),payload);
        }


        [Then(@"the service recipients are returned")]
        public async Task ThenTheServiceRecipientsAreReturned(Table table)
        {
            var expected = table.CreateSet<ServiceRecipientTable>();

            var payload = new ServiceRecipientsTable {ServiceRecipients = table.CreateSet<ServiceRecipientTable>() };

            var serviceRecipients = (await _response.ReadBodyAsJsonAsync()).SelectToken("serviceRecipients").Select(CreateServiceRecipients);

            expected.Should().BeEquivalentTo(serviceRecipients, conf => conf.Excluding(x => x.OrderDescription));
        }

        [Then(@"the persisted service recipients are")]
        public async Task ThenThePersistedServiceRecipientsAreReturned(Table table)
        {
            var expected = table.CreateSet<ServiceRecipientTable>();
            var actual = await ServiceRecipientEntity.FetchAllServiceRecipients(_settings.ConnectionString);
            var expectedWithOrderIds = expected.Select(s => new
            {
                OrderId = _context.GetOrderIdByDescription(s.OrderDescription), 
                OdsCode = s.OdsCode,
                Name = s.Name
            });
            expectedWithOrderIds.Should().BeEquivalentTo(actual);
        }

        private sealed class ServiceRecipientsTable
        {
            public IEnumerable<ServiceRecipientTable> ServiceRecipients { get; set; }
        }

        private static ServiceRecipientTable CreateServiceRecipients(JToken token)
        {
            return new ServiceRecipientTable
            {
                Name = token.SelectToken("name").ToString(),
                OdsCode = token.SelectToken("odsCode").ToString()
            };
        }

        private sealed class ServiceRecipientTable
        {
            public string OdsCode { get; set; }
            public string Name { get; set; }
            public string OrderDescription { get; set; }
        }
    }
}
