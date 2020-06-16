using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderDescriptionSteps
    {
        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;

        private readonly string _orderDescriptionUrl;

        public OrderDescriptionSteps(Response response, Request request, Settings settings, ScenarioContext context)
        {
            _context = context;
            _response = response;
            _request = request;
            _settings = settings;

            _orderDescriptionUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/description";
        }

        [When(@"the user makes a request to retrieve the order description section with the ID (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrdersDescriptionWithOrderId(string orderId)
        {
            await _request.GetAsync(string.Format(_orderDescriptionUrl, orderId));
        }

        [When(@"the user makes a request to retrieve the order description section for order with description (.*)")]
        public async Task WhenAGetRequestIsMadeForAnOrdersDescriptionSectionForOrderWithOrderDescription(string description)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            await _request.GetAsync(string.Format(_orderDescriptionUrl, orderId));
        }


        [Then(@"the order description is returned")]
        public async Task ThenTheOrderDescriptionIsReturned(Table table)
        {
            var expected = table.CreateSet<OrderDescriptionTable>().FirstOrDefault();

            var response = await _response.ReadBodyAsJsonAsync();

            var actual = new OrderDescriptionTable
            {
                Description = response.Value<string>("description")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [When(@"the user makes a request to update the description on the order with Description (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheDescriptionWithOrderId(string description, Table table)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            var data = table.CreateInstance<OrderDescriptionTable>();

            _context.SetOrderIdByDescription(data.Description, (int) orderId);
            await _request.PutJsonAsync(string.Format(_orderDescriptionUrl, orderId), data);
        }

        [When(@"the user makes a request to update the description with the ID (.*) with no model")]
        public async Task WhenTheUserMakesARequestToUpdateTheDescriptionWithOrderIdWithNoModel(string orderId)
        {
            await _request.PutJsonAsync(string.Format(_orderDescriptionUrl, orderId), null);
        }

        [Then(@"the order description for order with id (.*) is set to")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetTo(int orderId, Table table)
        {
            var expected = table.CreateInstance<OrderDescriptionTable>().Description;

            var actual = (await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId)).Description;
            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the order description (.*) is updated to (.*)")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsUpdatedTo(string preUpdateDescription, string postUpdateDescription)
        {
            var originalOrderId = _context.GetOrderIdByDescription(preUpdateDescription);
            originalOrderId.Should().NotBeNull("No stored mapping for given description");
            var actual = (await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, originalOrderId)).Description;
            actual.Should().BeEquivalentTo(postUpdateDescription);
        }

        [Then(@"the order with description (.*) is set to")]
        public async Task ThenTheOrderDescriptionForOrderWithIdIsSetTo(string preUpdateDescription, Table table)
        {
            var postUpdateDescription = table.CreateInstance<OrderDescriptionTable>().Description;
            var originalOrderId = _context.GetOrderIdByDescription(preUpdateDescription);
            originalOrderId.Should().NotBeNull("No stored mapping for given description");
            var actual = (await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, originalOrderId)).Description;
            actual.Should().BeEquivalentTo(postUpdateDescription);
        }



        [Then(@"the lastUpdatedName is updated in the database to (.*) with Order Description (.*)")]
        public async Task ThenTheLastUpdatedNameIsUpdatedInTheDatabase(string expected, string description)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            var actual = (await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId)).LastUpdatedByName;
            actual.Should().BeEquivalentTo(expected);
        }

        private sealed class OrderDescriptionTable
        {
            public string Description { get; set; }
        }
    }
}
