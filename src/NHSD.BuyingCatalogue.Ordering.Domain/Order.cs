using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Order
    {
        private readonly List<ServiceRecipient> _serviceRecipients;

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

            _serviceRecipients = new List<ServiceRecipient>();
        }

        public static Order Create(
            OrderDescription description, 
            Guid organisationId, 
            Guid lastUpdatedById, 
            string lastUpdatedByName)
        {
            var order = new Order(description, organisationId);

            order.ChangeLastUpdatedBy(lastUpdatedById, lastUpdatedByName);

            return order;
        }

        public string OrderId { get; set; }

        public OrderDescription Description { get; private set; }

        public Guid OrganisationId { get; private set; }

        public string OrganisationName { get; private set; }

        public string OrganisationOdsCode { get; private set; }

        public Address OrganisationAddress { get; private set; }

        public Contact OrganisationContact { get; private set; }

        public DateTime Created { get; }

        public DateTime LastUpdated { get; private set; }

        public Guid LastUpdatedBy { get; private set; }

        public string LastUpdatedByName { get; private set; }

        public OrderStatus OrderStatus { get; }

        public bool ServiceRecipientsViewed { get; set; }

        public bool CatalogueSolutionsViewed { get; private set; }

        public string SupplierId { get; private set; } 

        public string SupplierName { get; private set; }

        public Address SupplierAddress { get; private set; }

        public Contact SupplierContact { get; private set; }

        public DateTime? CommencementDate { get; private set; }

        public IReadOnlyCollection<ServiceRecipient> ServiceRecipients 
            => _serviceRecipients.AsReadOnly();

        public void ChangeDescription(OrderDescription orderDescription, Guid userId, string name)
        {
            Description = orderDescription ?? throw new ArgumentNullException(nameof(orderDescription));
            ChangeLastUpdatedBy(userId, name);
        }

        public void ChangeOrderParty(
            string orderingPartyName, 
            string orderingPartyOdsCode, 
            Address orderPartyAddress,
            Contact orderingPartyContact, 
            Guid userId, 
            string name)
        {
            OrganisationName = orderingPartyName;
            OrganisationOdsCode = orderingPartyOdsCode;
            OrganisationAddress = orderPartyAddress;
            OrganisationContact = orderingPartyContact;

            ChangeLastUpdatedBy(userId, name);
        }

        public void ChangeSupplier(
            string supplierId, 
            string supplierName, 
            Address supplierAddress, 
            Contact supplierContact, 
            Guid userId, 
            string name)
        {
            SupplierId = supplierId;
            SupplierName = supplierName;
            SupplierAddress = supplierAddress;
            SupplierContact = supplierContact;

            ChangeLastUpdatedBy(userId, name);
        }

        public void MarkCatalogueSolutionsAsViewed(Guid userId, string name)
        {
            if (CatalogueSolutionsViewed)
                return;

            CatalogueSolutionsViewed = true;
            ChangeLastUpdatedBy(userId, name);
        }

        private void ChangeLastUpdatedBy(Guid userId, string name)
        {
            LastUpdatedBy = userId;
            LastUpdatedByName = name ?? throw new ArgumentNullException(nameof(name));
            LastUpdated = DateTime.UtcNow;
        }

        public void ChangeCommencementDate(DateTime commencementDate, Guid userId, string name)
        {
            if (CommencementDate.GetValueOrDefault().Date.Equals(commencementDate.Date))
                return;

            CommencementDate = commencementDate;
            ChangeLastUpdatedBy(userId, name);
        }

        public void ChangeServiceRecipients(IList<ServiceRecipient> serviceRecipients, Guid userId, string name)
        {
            if (serviceRecipients is null)
                throw new ArgumentNullException(nameof(serviceRecipients));

            var newList = new List<ServiceRecipient>();
            var deleteList = _serviceRecipients.ToList();

            foreach (ServiceRecipient newServiceRecipient in serviceRecipients)
            {
                var serviceRecipient = _serviceRecipients.FirstOrDefault(item => newServiceRecipient.Equals(item));
                if (serviceRecipient is null)
                {
                    newList.Add(newServiceRecipient);
                }
                else
                {
                    deleteList.Remove(newServiceRecipient);
                }
            }

            foreach (ServiceRecipient serviceRecipient in deleteList)
            {
                _serviceRecipients.Remove(serviceRecipient);
            }

            foreach (ServiceRecipient newServiceRecipient in newList)
            {
                _serviceRecipients.Add(new ServiceRecipient
                {
                    OdsCode = newServiceRecipient.OdsCode,
                    Name = newServiceRecipient.Name,
                    Order = this
                });
            }

            ChangeLastUpdatedBy(userId, name);
        }
    }
}
