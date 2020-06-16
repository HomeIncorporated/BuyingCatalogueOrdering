using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderSteps
    {
        private readonly ScenarioContext _context;
        private readonly Request _request;
        private readonly Response _response;
        private readonly Settings _settings;

        private readonly string _orderOrganisationsUrl;

        public OrderSteps(
            ScenarioContext context,
            Request request, 
            Response response, 
            Settings settings)
        {
            _context = context;
            _request = request;
            _response = response;
            _settings = settings;

            _orderOrganisationsUrl = _settings.OrderingApiBaseUrl + "/api/v1/organisations/{0}/orders";
        }

        [Given(@"the order with orderId (.*) has a primary contact")]
        public async Task ThenTheOrderHasAPrimaryContact(int orderId)
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            order.OrganisationContactId.Should().NotBeNull();
        }

        [Given(@"the order with orderId (.*) does not have a primary contact")]
        public async Task ThenTheOrderDoesNotHaveAPrimaryContact(int orderId)
        {
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            order.OrganisationContactId.Should().BeNull();
        }

        [Given(@"Orders exist")]
        public async Task GivenOrdersExist(Table table)
        {
            IDictionary<string, int> orderDictionary = new Dictionary<string, int>();

            if (_context.ContainsKey(ScenarioContextKeys.OrderMapDictionary))
            {
                orderDictionary = (IDictionary<string, int>) _context[ScenarioContextKeys.OrderMapDictionary];
            }

            foreach (var ordersTableItem in table.CreateSet<OrdersTable>())
            {
                int? organisationAddressId = _context.GetAddressIdByPostcode(ordersTableItem.OrganisationAddressPostcode);
                int? organisationContactId = _context.GetContactIdByEmail(ordersTableItem.OrganisationContactEmail);

                int? supplierAddressId = _context.GetAddressIdByPostcode(ordersTableItem.SupplierAddressPostcode);
                int? supplierContactId = _context.GetContactIdByEmail(ordersTableItem.SupplierContactEmail);

                DateTime? commencementDate = null;
                if (ordersTableItem.CommencementDate != DateTime.MinValue)
                {
                    commencementDate = ordersTableItem.CommencementDate;
                }

                var order = OrderEntityBuilder
                    .Create()
                    .WithDescription(ordersTableItem.Description)
                    .WithOrganisationId(ordersTableItem.OrganisationId)
                    .WithOrganisationName(ordersTableItem.OrganisationName)
                    .WithOrganisationOdsCode(ordersTableItem.OrganisationOdsCode)
                    .WithOrganisationAddressId(organisationAddressId)
                    .WithOrganisationBillingAddressId(ordersTableItem.OrganisationBillingAddressId)
                    .WithOrganisationContactId(organisationContactId)
                    .WithOrderStatusId(ordersTableItem.OrderStatusId)
                    .WithDateCreated(ordersTableItem.Created != DateTime.MinValue ? ordersTableItem.Created : DateTime.UtcNow)
                    .WithLastUpdatedBy(ordersTableItem.LastUpdatedBy)
                    .WithLastUpdatedName(ordersTableItem.LastUpdatedByName)
                    .WithLastUpdated(ordersTableItem.LastUpdated != DateTime.MinValue ? ordersTableItem.LastUpdated : DateTime.UtcNow)
                    .WithServiceRecipientsViewed(ordersTableItem.ServiceRecipientsViewed)
                    .WithCatalogueSolutionsViewed(ordersTableItem.CatalogueSolutionsViewed)
                    .WithSupplierId(ordersTableItem.SupplierId)
                    .WithSupplierName(ordersTableItem.SupplierName)
                    .WithSupplierAddressId(supplierAddressId)
                    .WithSupplierContactId(supplierContactId)
                    .WithCommencementDate(commencementDate)
                    .Build();
                order.OrderId.Should().BeNull();
                var orderId = await order.InsertAsync<int>(_settings.ConnectionString);
                orderDictionary.Add(order.Description,orderId);
            }
            _context[ScenarioContextKeys.OrderMapDictionary] = orderDictionary;
        }

        [When(@"a GET request is made for a list of orders with organisationId (.*)")]
        public async Task WhenAGetRequestIsMadeForOrders(Guid organisationId)
        {
            await _request.GetAsync(string.Format(_orderOrganisationsUrl, organisationId));
        }

        [Then(@"the orders list is returned with the following values")]
        public async Task ThenTheOrdersListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrders = table.CreateSet<GetOrdersTable>();

            var orders = (await _response.ReadBodyAsJsonAsync()).Select(CreateOrders);

            orders.Should().BeEquivalentTo(expectedOrders,o=>o.Excluding(o=>o.OrderId));
        }

        [Then(@"an empty list is returned")]
        public async Task AnEmptyListIsReturned()
        {
            var orders = (await _response.ReadBodyAsJsonAsync()).Select(CreateOrders);
            orders.Count().Should().Be(0);
        }

        [Then(@"the order with description (.*) is updated in the database with data")]
        public async Task ThenTheOrderIsUpdatedInTheDatabase(string description, Table table)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            table.CompareToInstance(actual);
        }


        [Then(@"the order is created in the database with orderId (.*) and data")]
        public async Task ThenTheOrderIsCreatedInTheDatabase(int orderId, Table table)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            table.CompareToInstance(actual);
        }

        [Then(@"the order is created in the database with Description (.*) and data")]
        public async Task ThenTheOrderIsCreatedInTheDatabaseWithDescription(string description, Table table)
        {
            var actual = await OrderEntity.FetchOrderByDescription(_settings.ConnectionString, description);
            table.CompareToInstance(actual);
        }


        [Then(@"the order with description (.*) is updated and has a primary contact with data")]
        public async Task ThenTheOrderWithOrderIdHasContactData(string descripton, Table table)
        {
            var orderId = _context.GetOrderIdByDescription(descripton);

            var expected = table.CreateInstance<ContactEntity>();

            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            var actual = await ContactEntity.FetchContactById(_settings.ConnectionString, order.OrganisationContactId);

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the order with description (.*) is updated and has a Organisation Address with data")]
        public async Task ThenTheOrderWithOrderIdHasOrganisationAddresData(string description, Table table)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            var order = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            var actual = await AddressEntity.FetchAddressById(_settings.ConnectionString, order.OrganisationAddressId);
            table.CompareToInstance<AddressEntity>(actual);
        }

        [Then(@"the order with description (.*) has LastUpdated time present and it is the current time")]
        public async Task ThenOrderDescriptionHasLastUpdatedAtCurrentTime(string description)
        {
            var actual = await OrderEntity.FetchOrderByDescription(_settings.ConnectionString, description);
            actual.LastUpdated.Should().BeWithin(TimeSpan.FromSeconds(3)).Before(DateTime.UtcNow);
        }


        [Then(@"the order with orderId (.*) has Created time present and it is the current time")]
        public async Task ThenOrderOrderIdHasCreatedAtCurrentTime(int orderId)
        {
            var actual = await OrderEntity.FetchOrderByOrderId(_settings.ConnectionString, orderId);
            actual.Created.Should().BeWithin(TimeSpan.FromSeconds(3)).Before(DateTime.UtcNow);
        }

        [Then(@"the order with description (.*) has Created time present and it is the current time")]
        public async Task ThenOrderDescriptionHasCreatedAtCurrentTime(string description)
        {
            var actual = await OrderEntity.FetchOrderByDescription(_settings.ConnectionString, description);
            actual.Created.Should().BeWithin(TimeSpan.FromSeconds(3)).Before(DateTime.UtcNow);
        }


        private static object CreateOrders(JToken token)
        {
            return new
            {
                OrderId = token.Value<string>("orderId"),
                Description = token.Value<string>("description"),
                Status = token.Value<string>("status"),
                LastUpdated = token.Value<DateTime>("lastUpdated"),
                LastUpdatedByName = token.Value<string>("lastUpdatedBy"),
                Created = token.Value<DateTime>("dateCreated")
            };
        }

        private sealed class GetOrdersTable
        {
            public string OrderId { get; set; }

            public string Description { get; set; }

            public string Status { get; set; }

            public DateTime Created { get; set; }

            public DateTime LastUpdated { get; set; }

            public string LastUpdatedByName { get; set; }
        }

        private sealed class OrdersTable
        {
            public string OrderId { get; set; }

            public string Description { get; set; }

            public Guid OrganisationId { get; set; }

            public string OrganisationName { get; set; }

            public string OrganisationOdsCode { get; set; }

            public string OrganisationAddressPostcode { get; set; }

            public string OrganisationContactEmail { get; set; }

            public int? OrganisationBillingAddressId { get; set; }

            public int OrderStatusId { get; set; } = 1;

            public DateTime Created { get; set; }

            public Guid LastUpdatedBy { get; set; }

            public string LastUpdatedByName { get; set; }

            public DateTime LastUpdated { get; set; }

            public string SupplierId { get; set; }

            public string SupplierName { get; set; }

            public string SupplierAddressPostcode { get; set; }

            public string SupplierContactEmail { get; set; }

            public DateTime CommencementDate { get; set; }

            public bool ServiceRecipientsViewed { get; set; }

            public bool CatalogueSolutionsViewed { get; set; }
        }
    }
}
