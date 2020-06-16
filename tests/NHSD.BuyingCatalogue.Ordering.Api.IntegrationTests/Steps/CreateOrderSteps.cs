﻿using System;
using FluentAssertions;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class CreateOrderSteps
    {
        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly string _orderingUrl;

        public CreateOrderSteps(ScenarioContext context, Response response, Request request, Settings settings)
        {
            _context = context;
            _response = response;
            _request = request;
            _settings = settings;
            _orderingUrl = settings.OrderingApiBaseUrl + "/api/v1/orders";
        }

        [When(@"a POST request is made to create an order")]
        public async Task WhenAOrderIsCreated(Table table)
        {
            var data = table.CreateInstance<CreateOrderPayload>();
            await _request.PostJsonAsync(_orderingUrl, data);
        }

        [Then(@"a create order response is returned for order with description (.*)")]
        public async Task ThenTheCreateOrderResponseIsReturnedForOrderWithDescription(string description)
        {
            var order = await OrderEntity.FetchOrderByDescription(_settings.ConnectionString, description);
            order.Should().NotBeNull();
            var responseOrderId = (await _response.ReadBodyAsJsonAsync()).Value<int>("orderId");
            order.OrderId.Should().Be(responseOrderId);
        }

        private sealed class CreateOrderPayload
        {
            public Guid OrganisationId { get; set; }

            public string Description { get; set; }
        }
    }
}
