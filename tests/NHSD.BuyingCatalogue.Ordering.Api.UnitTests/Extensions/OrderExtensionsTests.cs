﻿using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class OrderExtensionsTests
    {
        [Test]
        public void IsSupplierSectionComplete_HasPrimaryContact_ReturnsTrue()
        {
            var order = OrderBuilder
                .Create()
                .WithSupplierContact(ContactBuilder.Create().Build())
                .Build();

            order.IsSupplierSectionComplete().Should().BeTrue();
        }

        [Test]
        public void IsSupplierSectionComplete_NullPrimaryContact_ReturnsFalse()
        {
            var order = OrderBuilder
                .Create()
                .WithSupplierContact(null)
                .Build();

            order.IsSupplierSectionComplete().Should().BeFalse();
        }

        [Test]
        public void IsSupplierSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsSupplierSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsCommencementDateSectionComplete_HasCommencementDate_ReturnsTrue()
        {
            var order = OrderBuilder
                .Create()
                .WithCommencementDate(new DateTime(2020, 05, 31))
                .Build();

            order.IsCommencementDateSectionComplete().Should().BeTrue();
        }

        [Test]
        public void IsCommencementDateSectionComplete_NullPrimaryContact_ReturnsFalse()
        {
            var order = OrderBuilder
                .Create()
                .WithCommencementDate(null)
                .Build();

            order.IsCommencementDateSectionComplete().Should().BeFalse();
        }

        [Test]
        public void IsCommencementDateSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsCommencementDateSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsServiceRecipientsSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsServiceRecipientsSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsServiceRecipientsSectionComplete_ServiceRecipientsViewed_ReturnsTrue()
        {
            var order = OrderBuilder.Create().WithServiceRecipientsViewed(true).Build();
            var actual = OrderExtensions.IsServiceRecipientsSectionComplete(order);
            actual.Should().BeTrue();
        }

        [Test]
        public void IsServiceRecipientsSectionComplete_ServiceRecipientsViewedFalse_ReturnsFalse()
        {
            var order = OrderBuilder.Create().WithServiceRecipientsViewed(false).Build();
            var actual = OrderExtensions.IsServiceRecipientsSectionComplete(order);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsCatalogueSolutionsSectionComplete_NullOrder_ReturnsFalse()
        {
            var actual = OrderExtensions.IsCatalogueSolutionsSectionComplete(null);
            actual.Should().BeFalse();
        }

        [Test]
        public void IsCatalogueSolutionsSectionComplete_CatalogueSolutionsViewed_ReturnsTrue()
        {
            var order = OrderBuilder.Create().WithCatalogueSolutionsViewed(true).Build();
            var actual = OrderExtensions.IsCatalogueSolutionsSectionComplete(order);
            actual.Should().BeTrue();
        }

        [Test]
        public void IsCatalogueSolutionsSectionComplete_CatalogueSolutionsNotViewed_ReturnsFalse()
        {
            var order = OrderBuilder.Create().WithCatalogueSolutionsViewed(false).Build();
            var actual = OrderExtensions.IsCatalogueSolutionsSectionComplete(order);
            actual.Should().BeFalse();
        }
    }
}
