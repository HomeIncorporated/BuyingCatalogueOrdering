﻿using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class GetCatalogueSolutionOrderItemModel
    {
        public ServiceRecipientModel ServiceRecipient { get; set; }

        public string CatalogueSolutionId { get; set; }

        public string CatalogueSolutionName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int Quantity { get; set; }

        public string EstimationPeriod { get; set; }

        public string ProvisioningType { get; set; }

        public string Type { get; set; }

        public string CurrencyCode { get; set; }

        public ItemUnitModel ItemUnit { get; set; }

        public decimal? Price { get; set; }
    }
}
