using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Order
    {
        private readonly List<ServiceRecipient> _serviceRecipients = new List<ServiceRecipient>();

        private Order()
        {
        }

        private Order(
            OrderDescription orderDescription,
            Guid organisationId) : this()
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
            OrganisationId = organisationId;
            OrderStatus = OrderStatus.Unsubmitted;
            Created = DateTime.UtcNow;
        }

        public static Order Create(OrderDescription description, Guid organisationId) 
            => new Order(description, organisationId);

        public string OrderId { get; set; }

        public OrderDescription Description { get; private set; }

        public Guid OrganisationId { get; }

        public string OrganisationName { get; private set; }

        public string OrganisationOdsCode { get; private set; }

        public int? OrganisationAddressId { get; }

        public Address OrganisationAddress { get; private set; }

        public int? OrganisationContactId { get; }

        public Contact OrganisationContact { get; private set; }

        public DateTime Created { get; }

        public DateTime LastUpdated { get; }

        public Guid LastUpdatedBy { get; }

        public string LastUpdatedByName { get; }

        public OrderStatus OrderStatus { get; }

        public bool ServiceRecipientsViewed { get; set; }

        public bool CatalogueSolutionsViewed { get; private set; }

        public string SupplierId { get; private set; }

        public string SupplierName { get; private set; }

        public int? SupplierAddressId { get; }

        public Address SupplierAddress { get; private set; }

        public int? SupplierContactId { get; }

        public Contact SupplierContact { get; private set; }

        public DateTime? CommencementDate { get; private set; }

        public IReadOnlyCollection<ServiceRecipient> ServiceRecipients
            => _serviceRecipients.AsReadOnly();

        public void ChangeDescription(OrderDescription orderDescription)
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
        }

        public void ChangeOrderParty(
            string orderingPartyName,
            string orderingPartyOdsCode,
            Address orderPartyAddress,
            Contact orderingPartyContact)
        {
            OrganisationName = orderingPartyName;
            OrganisationOdsCode = orderingPartyOdsCode;
            OrganisationAddress = orderPartyAddress;
            OrganisationContact = orderingPartyContact;
        }

        public void ChangeSupplier(
            string supplierId,
            string supplierName,
            Address supplierAddress,
            Contact supplierContact)
        {
            SupplierId = supplierId;
            SupplierName = supplierName;
            SupplierAddress = supplierAddress;
            SupplierContact = supplierContact;
        }

        private void MarkServiceRecipientsAsViewed()
        {
            if (ServiceRecipientsViewed)
                return;

            ServiceRecipientsViewed = true;
        }

        public void MarkCatalogueSolutionsAsViewed()
        {
            if (CatalogueSolutionsViewed)
                return;

            CatalogueSolutionsViewed = true;
        }

        public void ChangeCommencementDate(DateTime commencementDate)
        {
            if (CommencementDate.GetValueOrDefault().Date.Equals(commencementDate.Date))
                return;

            CommencementDate = commencementDate;
        }

        public void ChangeServiceRecipients(IList<(string odsCode, string name)> serviceRecipients)
        {
            if (serviceRecipients is null)
                throw new ArgumentNullException(nameof(serviceRecipients));

            _serviceRecipients.Clear();

            foreach ((string odsCode, string name) in serviceRecipients)
            {
                _serviceRecipients.Add(ServiceRecipient.Create(odsCode, name, this));
            }

            MarkServiceRecipientsAsViewed();

            CatalogueSolutionsViewed = (!_serviceRecipients.Any());
        }

        private bool Equals(Order other)
        {
            return OrderId == other.OrderId;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Order other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId);
        }
    }
}
