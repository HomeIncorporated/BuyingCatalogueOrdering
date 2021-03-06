﻿using System;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Requests;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Responses;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class GetOrderRequestSteps
    {
        private readonly Request _request;
        private readonly Settings _settings;
        private readonly OrderContext _orderContext;
        private GetOrderRequest _getOrderRequest;
        private GetOrderResponse _getOrderResponse;

        public GetOrderRequestSteps(
            Request request, 
            Settings settings,
            OrderContext orderContext)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _orderContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
        }

        [Given(@"the user creates a request to retrieve the details of an order by ID '(.*)'")]
        public void GivenTheUserCreatesARequestToRetrieveTheDetailsOfAnOrderById(string orderId)
        {
            _getOrderRequest = new GetOrderRequest(
                _request,
                _settings.OrderingApiBaseUrl,
                orderId);
        }
        
        [When(@"the user sends the get order request")]
        public async Task WhenTheUserSendsTheGetOrderRequest()
        {
            _getOrderResponse = await _getOrderRequest.ExecuteAsync();
        }
        
        [Then(@"the get order response displays the expected order")]
        public async Task ThenTheGetOrderResponseDisplaysTheExpectedOrder()
        {
            var order = _orderContext.OrderReferenceList.GetByOrderId(_getOrderRequest.OrderId);
            var orderItems = _orderContext.OrderItemReferenceList.FindByOrderId(_getOrderRequest.OrderId);
            var serviceRecipients = _orderContext.ServiceRecipientReferenceList.FindByOrderId(_getOrderRequest.OrderId);

            await _getOrderResponse.AssertAsync(order, orderItems, serviceRecipients);
        }
    }
}
