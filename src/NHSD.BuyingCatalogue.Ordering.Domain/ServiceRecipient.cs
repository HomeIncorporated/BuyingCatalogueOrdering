using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ServiceRecipient
    {
        public string OdsCode { get; }

        public string OrderId { get; }

        public Order Order { get; private set; }

        public string Name { get; }

        public ServiceRecipient(string odsCode, string name)
        {
            OdsCode = odsCode ?? throw new ArgumentNullException(nameof(odsCode));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        internal void SetOrder(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }

        private bool Equals(ServiceRecipient other)
        {
            return OdsCode == other.OdsCode && OrderId == other.OrderId;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ServiceRecipient other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OdsCode, OrderId);
        }
    }
}
