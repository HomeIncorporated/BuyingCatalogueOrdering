using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    public sealed class CreateOrderService : ICreateOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Result<string>> CreateAsync(CreateOrderRequest createOrderRequest)
        {
            if (createOrderRequest is null)
            {
                throw new ArgumentNullException(nameof(createOrderRequest));
            }

            var descriptionResult = OrderDescription.Create(createOrderRequest.Description);
            var orderOrganisationIdResult = OrderOrganisationId.Create(createOrderRequest.OrganisationId);

            if (!descriptionResult.IsSuccess || !orderOrganisationIdResult.IsSuccess)
            {
                var allErrors = descriptionResult.Errors.Union(orderOrganisationIdResult.Errors);
                return Result.Failure<string>(allErrors);
            }

            var order = Order.Create(
                descriptionResult.Value,
                orderOrganisationIdResult.Value);

            var orderId = await _orderRepository.CreateOrderAsync(order);
            return Result.Success(orderId);
        }
    }
}
