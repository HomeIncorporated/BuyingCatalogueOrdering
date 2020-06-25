﻿using System;
using System.Threading;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests.Builders
{
    internal sealed class OrderBuilder
    {
        private Guid _lastUpdatedBy;
        private string _lastUpdatedByName;

        /// <summary>
        /// Initialises a new instance of the <see cref="OrderBuilder"/> class.
        /// </summary>
        private OrderBuilder()
        {
            _lastUpdatedBy = Guid.NewGuid();
            _lastUpdatedByName = "Bob Smith";
        }

        public static OrderBuilder Create() => 
            new OrderBuilder();

        public OrderBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _lastUpdatedBy = lastUpdatedBy;
            return this;
        }

        public OrderBuilder WithLastUpdatedByName(string lastUpdatedByName)
        {
            _lastUpdatedByName = lastUpdatedByName;
            return this;
        }

        public Order Build()
        {
            var order = new Order
            {
                LastUpdatedBy = _lastUpdatedBy,
                LastUpdatedByName = _lastUpdatedByName
            };

            return order;
        }
    }
}
