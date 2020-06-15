﻿using System;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class OrderBuilder
    {

        private string _orderId;
        private string _orderDescription;
        private Guid _organisationId;
        private readonly string _organisationName;
        private readonly string _organisationOdsCode;
        private readonly Address _organisationAddress;
        private Contact _organisationContact;
        private DateTime _created;
        private DateTime _lastUpdated;
        private Guid _lastUpdatedBy;
        private string _lastUpdatedByName;
        private string _supplierId;
        private string _supplierName;
        private Address _supplierAddress;
        private Contact _supplierContact;
        private DateTime? _commencementDate;
        private bool _serviceRecipientsViewed;
        private bool _catalogueSolutionsViewed;

        private OrderBuilder()
        {
            _orderId = "C000014-01";
            _orderDescription = "Some Description";
            _organisationId = Guid.NewGuid();
            _organisationName = "Organisation Name";
            _organisationOdsCode = "Ods Code";
            _organisationAddress = AddressBuilder.Create().WithLine1("1 Some Ordering Party").Build();
            _organisationContact = null;
            _created = DateTime.UtcNow;
            _lastUpdated = DateTime.UtcNow;
            _lastUpdatedBy = Guid.NewGuid();
            _lastUpdatedByName = "Bob Smith";
            _supplierId = "Some supplier id";
            _supplierName = "Some supplier name";
            _supplierAddress = AddressBuilder.Create().WithLine1("1 Some Supplier").Build();
            _supplierContact = null;
            _commencementDate = null;
            _serviceRecipientsViewed = false;
            _catalogueSolutionsViewed = false;
        }

        internal static OrderBuilder Create() => new OrderBuilder();

        internal OrderBuilder WithOrderId(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        internal OrderBuilder WithDescription(string description)
        {
            _orderDescription = description;
            return this;
        }

        internal OrderBuilder WithOrganisationId(Guid organisationId)
        {
            _organisationId = organisationId;
            return this;
        }

        internal OrderBuilder WithOrganisationContact(Contact organisationContact)
        {
            _organisationContact = organisationContact;
            return this;
        }

        internal OrderBuilder WithSupplierId(string supplierId)
        {
            _supplierId = supplierId;
            return this;
        }

        internal OrderBuilder WithSupplierName(string supplierName)
        {
            _supplierName = supplierName;
            return this;
        }

        internal OrderBuilder WithSupplierAddress(Address supplierAddress)
        {
            _supplierAddress = supplierAddress;
            return this;
        }

        internal OrderBuilder WithSupplierContact(Contact supplierContact)
        {
            _supplierContact = supplierContact;
            return this;
        }

        internal OrderBuilder WithCommencementDate(DateTime? commencementDate)
        {
            _commencementDate = commencementDate;
            return this;
        }

        internal OrderBuilder WithServiceRecipientsViewed(bool serviceRecipientsViewed)
        {
            _serviceRecipientsViewed = serviceRecipientsViewed;
            return this;
        }

        internal OrderBuilder WithCatalogueSolutionsViewed(bool catalogueSolutionsViewed)
        {
            _catalogueSolutionsViewed = catalogueSolutionsViewed;
            return this;
        }

        internal OrderBuilder WithLastUpdatedBy(Guid lastUpdatedBy)
        {
            _lastUpdatedBy = lastUpdatedBy;
            return this;
        }

        public OrderBuilder WithLastUpdatedByName(string lastUpdatedByName)
        {
            _lastUpdatedByName = lastUpdatedByName;
            return this;
        }

        internal Order Build()
        {
            var descriptionResult = OrderDescription.Create(_orderDescription);

            var order = Order.Create(descriptionResult.Value, _organisationId, _lastUpdatedBy, _lastUpdatedByName);

            order.OrderId = _orderId;

            order.ChangeOrderParty(
                _organisationName,
                _organisationOdsCode,
                _organisationAddress,
                _organisationContact,
                _lastUpdatedBy,
                _lastUpdatedByName);

            order.ChangeSupplier(
                _supplierId,
                _supplierName,
                _supplierAddress,
                _supplierContact,
                _lastUpdatedBy,
                _lastUpdatedByName);

            if (_commencementDate.HasValue)
            {
                order.ChangeCommencementDate(_commencementDate.Value, _lastUpdatedBy, _lastUpdatedByName);
            }

            order.ServiceRecipientsViewed = _serviceRecipientsViewed;

            if (_catalogueSolutionsViewed)
            {
                order.MarkCatalogueSolutionsAsViewed(_lastUpdatedBy, _lastUpdatedByName);
            }

            return order;
        }
    }
}
