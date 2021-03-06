﻿using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class UpdateOrderItemModel
    {
        public DateTime? DeliveryDate { get; set; }

        public int Quantity { get; set; }

        public string EstimationPeriod { get; set; }

        public decimal? Price { get; set; }
    }
}
