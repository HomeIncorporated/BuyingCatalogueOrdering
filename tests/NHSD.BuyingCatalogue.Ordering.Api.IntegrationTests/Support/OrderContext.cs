﻿namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Support
{
    internal sealed class OrderContext
    {
        public OrderContext()
        {
            OrderReferenceList = new OrderReferenceList();
            OrderItemReferenceList = new OrderItemReferenceList();
        }

        public OrderReferenceList OrderReferenceList { get; }

        public OrderItemReferenceList OrderItemReferenceList { get; }
    }
}
