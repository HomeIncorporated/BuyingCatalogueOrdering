﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Api.Models.Summary;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;
using NHSD.BuyingCatalogue.Ordering.Common.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = PolicyName.CanAccessOrders)]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICreateOrderService _createOrderService;
        private readonly IServiceRecipientRepository _serviceRecipientRepository;

        public OrdersController(
            IOrderRepository orderRepository,
            ICreateOrderService createOrderService,
            IServiceRecipientRepository serviceRecipientRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _createOrderService = createOrderService ?? throw new ArgumentNullException(nameof(createOrderService));
            _serviceRecipientRepository = serviceRecipientRepository ?? throw new ArgumentNullException(nameof(serviceRecipientRepository));
        }

        [HttpGet]
        [Route("{orderId}")]
        public async Task<ActionResult<OrderModel>> GetAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound();
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            return new OrderModel
            {
                Description = order.Description.Value,
                OrderParty = new OrderingPartyModel
                {
                    Name = order.OrganisationName,
                    OdsCode = order.OrganisationOdsCode,
                    Address = order.OrganisationAddress.ToModel(),
                    PrimaryContact = order.OrganisationContact.ToModel()
                },
                CommencementDate = order.CommencementDate,
                Supplier = new SupplierModel
                {
                    Name = order.SupplierName,
                    Address = order.SupplierAddress.ToModel(),
                    PrimaryContact = order.SupplierContact.ToModel()
                },
                TotalOneOffCost = 0m,
                TotalRecurringCostPerMonth = 0m,
                TotalRecurringCostPerYear = 0m,
                TotalOwnershipCost = 0m,
                ServiceRecipients = order.ServiceRecipients.Select(serviceRecipient =>
                    new ServiceRecipientModel
                    {
                        Name = serviceRecipient.Name,
                        OdsCode = serviceRecipient.OdsCode
                    }),
                OrderItems = order.OrderItems.Select(orderItem =>
                    new OrderItemModel
                    {
                        ItemId = $"{order.OrderId}-{orderItem.OdsCode}-{orderItem.OrderItemId}",
                        ServiceRecipientsOdsCode = orderItem.OdsCode,
                        CataloguePriceType = orderItem.CataloguePriceType.Name,
                        CatalogueItemType = orderItem.CatalogueItemType.Name,
                        CatalogueItemName = orderItem.CatalogueItemName,
                        ProvisioningType = orderItem.ProvisioningType.Name,
                        ItemUnitDescription = orderItem.CataloguePriceUnit.Description,
                        Price = orderItem.Price,
                        Quantity = orderItem.Quantity,
                        CostPerYear = 0m
                    }),
            };
        }

        [HttpGet]
        [Route("/api/v1/organisations/{organisationId}/[controller]")]
        public async Task<ActionResult> GetAllAsync(Guid organisationId)
        {
            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != organisationId)
            {
                return Forbid();
            }

            var orders = await _orderRepository.ListOrdersByOrganisationIdAsync(organisationId);
            var orderModelResult = orders.Select(order => new OrderListItemModel
            {
                OrderId = order.OrderId,
                Description = order.Description.Value,
                LastUpdatedBy = order.LastUpdatedByName,
                LastUpdated = order.LastUpdated,
                DateCreated = order.Created,
                Status = order.OrderStatus.Name
            })
                .ToList();

            return Ok(orderModelResult);
        }

        [HttpGet]
        [Route("{orderId}/summary")]
        public async Task<ActionResult<OrderSummaryModel>> GetOrderSummaryAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound();
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            int serviceRecipientsCount = await _serviceRecipientRepository.GetCountByOrderIdAsync(orderId);
            int catalogueSolutionsCount = order.OrderItems.Count(y => y.CatalogueItemType.Equals(CatalogueItemType.Solution));

            OrderSummaryModel orderSummaryModel = new OrderSummaryModel
            {
                OrderId = orderId,
                OrganisationId = order.OrganisationId,
                Description = order.Description.Value,
                Sections = new List<SectionModel>
                {
                    SectionModel.Description,
                    SectionModel.OrderingParty.WithStatus(order.IsOrderingPartySectionComplete() ? "complete" : "incomplete"),
                    SectionModel.Supplier.WithStatus(order.IsSupplierSectionComplete() ? "complete" : "incomplete"),
                    SectionModel.CommencementDate.WithStatus(order.IsCommencementDateSectionComplete() ? "complete" : "incomplete"),
                    SectionModel.AssociatedServices,
                    SectionModel
                        .ServiceRecipients
                        .WithStatus(order.IsServiceRecipientsSectionComplete() ? "complete" : "incomplete")
                        .WithCount(serviceRecipientsCount),
                    SectionModel.CatalogueSolutions
                        .WithStatus(order.IsCatalogueSolutionsSectionComplete() ? "complete" : "incomplete")
                        .WithCount(catalogueSolutionsCount),
                    SectionModel.AdditionalServices,
                    SectionModel.FundingSource
                }
            };

            return Ok(orderSummaryModel);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.CanManageOrders)]
        public async Task<ActionResult<CreateOrderResponseModel>> CreateOrderAsync([FromBody][Required] CreateOrderModel order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var primaryOrganisationId = User.GetPrimaryOrganisationId();
            if (primaryOrganisationId != order.OrganisationId)
            {
                return Forbid();
            }

            var result = await _createOrderService.CreateAsync(new CreateOrderRequest
            {
                LastUpdatedByName = User.GetUserName(),
                LastUpdatedById = User.GetUserId(),
                Description = order.Description,
                OrganisationId = order.OrganisationId,
            });

            var createOrderResponse = new CreateOrderResponseModel();

            if (result.IsSuccess)
            {
                createOrderResponse.OrderId = result.Value;
                return CreatedAtAction(nameof(CreateOrderAsync).TrimAsync(), null, new { orderId = result.Value }, createOrderResponse);
            }

            createOrderResponse.Errors = result.Errors.Select(x => new ErrorModel(x.Id, x.Field));
            return BadRequest(createOrderResponse);
        }
    }
}
