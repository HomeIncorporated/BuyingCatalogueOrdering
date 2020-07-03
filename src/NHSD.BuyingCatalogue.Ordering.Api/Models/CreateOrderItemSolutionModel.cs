﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.BuyingCatalogue.Ordering.Api.Attributes;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public class CreateOrderItemSolutionModel : CreateOrderItemBaseModel
    {
        [Required(ErrorMessage = "CatalogueSolutionIdRequired")]
        [MaxLength(14, ErrorMessage = "CatalogueSolutionIdTooLong")]
        public string CatalogueSolutionId { get; set; }

        [Required(ErrorMessage = "CatalogueSolutionNameRequired")]
        [MaxLength(255, ErrorMessage = "CatalogueSolutionNameTooLong")]
        public string CatalogueSolutionName { get; set; }

        [Required(ErrorMessage = "DeliveryDateRequired")]
        public DateTime? DeliveryDate { get; set; }

        [OrderItemRequired("Declarative", "Patient", ErrorMessage = "TimeUnitRequired")]
        public string TimeUnit { get; set; }
    }
}