using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Controllers;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class ServiceRecipientsSectionControllerTests
    {
        [Test]
        public void Ctor_NullRepository_Throws()
        {
            static void Test()
            {
                var _ = new ServiceRecipientsSectionController(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [TestCase(null)]
        [TestCase("INVALID")]
        public async Task GetAllAsync_OrderDoesNotExist_ReturnsNotFound(string orderId)
        {
            var context = ServiceRecipientsTestContext.Setup();
            context.Order = null;

            var response = await context.Controller.GetAllAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(new NotFoundResult()));
        }

        [Test]
        public async Task GetAllAsync_OrganisationIdDoesNotMatch_ReturnsForbidden()
        {
            var context = ServiceRecipientsTestContext.Setup();
            context.Order = OrderBuilder.Create().Build();

            var response = await context.Controller.GetAllAsync("myOrder");

            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(new ForbidResult()));
        }

        [Test]
        public async Task GetAllAsync_NoServiceRecipient_ReturnsEmptyList()
        {
            var context = ServiceRecipientsTestContext.Setup();
            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = new List<ServiceRecipientModel>()
            };

            var response = await context.Controller.GetAllAsync("myOrder");
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(expected));
        }

        [Test]
        public async Task GetAllAsync_SingleServiceRecipient_ReturnsTheRecipient()
        {
            var context = ServiceRecipientsTestContext.Setup();

            const string orderId = "C0000014-01";

            var serviceRecipients = new List<(ServiceRecipient serviceRecipient, ServiceRecipientModel expectedModel)>
            {
                CreateServiceRecipientData("ODS1", orderId)
            };

            context.ServiceRecipients = serviceRecipients.Select(x => x.serviceRecipient).ToList();

        
            var expectedList = serviceRecipients.Select(x => x.expectedModel);

            var expected = new ServiceRecipientsModel
            {
                ServiceRecipients = expectedList
            };

            var response = await context.Controller.GetAllAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(expected));
        }
        
        [Test]
        public async Task GetAllAsync_MultipleServiceRecipientsMatch_ReturnsAllTheOrdersServicesRecipients()
        {
            var context = ServiceRecipientsTestContext.Setup();

            const string orderId = "C0000014-01";

            context.Order = OrderBuilder.Create().WithOrderId(orderId).WithOrganisationId(context.PrimaryOrganisationId).Build();

            var serviceRecipients = new List<(ServiceRecipient serviceRecipient, ServiceRecipientModel expectedModel)>
            {
                CreateServiceRecipientData("ODS1", orderId),
                CreateServiceRecipientData("ODS2", orderId),
                CreateServiceRecipientData("ODS3", orderId)
            };

            context.ServiceRecipients = serviceRecipients.Select(x => x.serviceRecipient).ToList();
            var expected = new ServiceRecipientsModel();

            var expectedList = serviceRecipients.Select(x => x.expectedModel);

            expected.ServiceRecipients = expectedList;

            var response = await context.Controller.GetAllAsync(orderId);
            response.Should().BeEquivalentTo(new ActionResult<ServiceRecipientsModel>(expected));
        }

        private static (ServiceRecipient serviceRecipient, ServiceRecipientModel expectedModel)
            CreateServiceRecipientData(string odsCode, string orderId)
        {
            var serviceRecipient = ServiceRecipientBuilder
                .Create()
                .WithOdsCode(odsCode)
                .WithOrderId(orderId)
                .Build();

            return (serviceRecipient,
                new ServiceRecipientModel { OdsCode = serviceRecipient.OdsCode, Name = serviceRecipient.Name });
        }

        private sealed class ServiceRecipientsTestContext
        {
            private ServiceRecipientsTestContext()
            {
                PrimaryOrganisationId = Guid.NewGuid();
                Order = OrderBuilder.Create().WithOrganisationId(PrimaryOrganisationId).Build();

                OrderRepositoryMock = new Mock<IOrderRepository>();
                OrderRepositoryMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<string>())).ReturnsAsync(() => Order);

                ServiceRecipients = new List<ServiceRecipient>();

                ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("Ordering", "Manage"),
                    new Claim("primaryOrganisationId", PrimaryOrganisationId.ToString()),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                },
                "mock"));

                Controller = new ServiceRecipientsSectionController(OrderRepositoryMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = ClaimsPrincipal }
                    }
                };
            }

            internal Guid PrimaryOrganisationId { get; }

            internal Mock<IOrderRepository> OrderRepositoryMock { get; }

            internal Order Order { get; set; }

            internal IEnumerable<ServiceRecipient> ServiceRecipients { get; set; }

            internal ServiceRecipientsSectionController Controller { get; }

            private ClaimsPrincipal ClaimsPrincipal { get; }

            internal static ServiceRecipientsTestContext Setup()
            {
                return new ServiceRecipientsTestContext();
            }
        }
    }
}
