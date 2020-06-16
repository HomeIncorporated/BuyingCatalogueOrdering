using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class OrderingPartySteps
    {
        private readonly Response _response;
        private readonly Request _request;
        private readonly string _orderingPartyUrl;
        private readonly ScenarioContext _context;

        public OrderingPartySteps(Response response, Request request, Settings settings, ScenarioContext context)
        {
            _response = response;
            _request = request;
            _context = context;
            _orderingPartyUrl = settings.OrderingApiBaseUrl + "/api/v1/orders/{0}/sections/ordering-party";

        }

        private OrganisationPartyPayload GetOrganisationPartyPayloadByOrderId(ScenarioContext context, int orderId)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var payloadDictionary =
                context.Get<IDictionary<int, OrganisationPartyPayload>>(ScenarioContextKeys.OrganisationPayloadDictionary, new Dictionary<int, OrganisationPartyPayload>());

            if (payloadDictionary.TryGetValue(orderId, out var payload))
                return payload;

            return null;
        }

        private void SetOrganisationPartyPayloadByOrderId(ScenarioContext context,int orderId, OrganisationPartyPayload payload)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (payload == null)
                return ;

            var payloadDictionary =
                context.Get<IDictionary<int, OrganisationPartyPayload>>(ScenarioContextKeys.OrganisationPayloadDictionary, new Dictionary<int, OrganisationPartyPayload>());

            payloadDictionary[orderId] = payload;

            if (!context.ContainsKey(ScenarioContextKeys.OrganisationPayloadDictionary))
            {
                context.Add(ScenarioContextKeys.OrganisationPayloadDictionary, payloadDictionary);
            }
        }

        [Given(@"an order party update request exist for order with description (.*)")]
        public void GivenAnOrderPartyUpdateRequestExistForOrderDescription(string description)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            SetOrganisationPartyPayloadByOrderId(_context, orderId ?? -999, new OrganisationPartyPayload());
        }

        [Given(@"the update request for order with description (.*) has a contact")]
        public void GivenTheUpdateRequestForOrderDescriptionHasAContact(string description, Table table)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            if (orderId != null)
            {
                var payload = GetOrganisationPartyPayloadByOrderId(_context, (int)orderId);
                payload.PrimaryContact = table.CreateInstance<ContactPayload>();
            }
        }

        [Given(@"the order party update request for order with description (.*) has a address")]
        public void GivenTheOrderPartyUpdateRequestForOrderIdHasAAddress(string description, Table table)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            if (orderId != null)
            {
                var payload = GetOrganisationPartyPayloadByOrderId(_context, (int)orderId);
                payload.Address = table.CreateInstance<AddressPayload>();
            }
        }

        [Given(@"the order party update request for order with description (.*) has a Name of (.*)")]
        public void GivenTheOrderPartyUpdateRequestForOrderDescriptionHasANameOfTestCareCenter(string description, string name)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            if (orderId != null)
            {
                var payload = GetOrganisationPartyPayloadByOrderId(_context, (int)orderId);
                payload.Name = name;
            }
        }

        [Given(@"the order party update request for order with description (.*) has a OdsCode of (.*)")]
        public void GivenTheOrderPartyUpdateRequestForOrderDescriptionHasAOrganisationOdsCodeOfTestCareOds(string description, string odsCode)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            if (orderId != null)
            {
                var payload = GetOrganisationPartyPayloadByOrderId(_context, (int)orderId);
                payload.OdsCode = odsCode;
            }
        }

        [When(@"the user makes a request to retrieve the ordering-party section for the order with description (.*)")]
        public async Task GivenTheUserMakesARequestToRetrieveTheOrdering_PartySectionWithTheOrderDescription(string description)
        {
            var orderId  =_context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            await _request.GetAsync(string.Format(_orderingPartyUrl, orderId));
        }


        [When(@"the user makes a request to retrieve the ordering-party section with unknown orderId")]
        public async Task GivenTheUserMakesARequestToRetrieveTheOrdering_PartySectionWithTheID()
        {
            await _request.GetAsync(string.Format(_orderingPartyUrl, -999));
        }

        [When(@"the user makes a request to update the order party on the order with the Description (.*)")]
        public async Task WhenTheUserMakesARequestToUpdateTheOrderPartyWithOrderId(string description)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            orderId.Should().NotBeNull();
            if (orderId != null)
            {
                var payload = GetOrganisationPartyPayloadByOrderId(_context,(int) orderId);
                await _request.PutJsonAsync(string.Format(_orderingPartyUrl, orderId), payload);
            }
        }

        [When(@"the user makes a request to update the order party for order with description (.*) with no model")]
        public async Task WhenTheUserMakesARequestToUpdateTheOrderPartyWithOrderIdWithNoModel(string description)
        {
            var orderId = _context.GetOrderIdByDescription(description);
            await _request.PutJsonAsync(string.Format(_orderingPartyUrl, orderId), null);
        }

        [Then(@"the ordering-party is returned")]
        public async Task ThenTheOrdering_PartyOrganisationIsReturned(Table table)
        {
            var expected = table.CreateSet<OrganisationTable>().FirstOrDefault();

            var response = (await _response.ReadBodyAsJsonAsync());

            var actual = new OrganisationTable
            {
                Name = response.Value<string>("name"),
                OdsCode = response.Value<string>("odsCode")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        private sealed class OrganisationTable
        {
            public string Name { get; set; }
            public string OdsCode { get; set; }
        }

        private sealed class OrganisationPartyPayload
        {
            public string Name { get; set; }
            public string OdsCode { get; set; }
            public AddressPayload Address { get; set; }
            public ContactPayload PrimaryContact { get; set; }
        }

        private sealed class AddressPayload
        {
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Line3 { get; set; }
            public string Line4 { get; set; }
            public string Line5 { get; set; }
            public string Town { get; set; }
            public string County { get; set; }
            public string Postcode { get; set; }
            public string Country { get; set; }
        }

        private sealed class ContactPayload
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public string TelephoneNumber { get; set; }
        }
    }
}
