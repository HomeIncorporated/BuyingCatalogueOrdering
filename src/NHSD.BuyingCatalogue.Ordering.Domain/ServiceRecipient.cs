using System;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class ServiceRecipient
    {
        public string OdsCode { get; }

        public string Name { get; }

        public string OrderId { get; }

        public Order Order { get; }

        private ServiceRecipient()
        {
        }

        private ServiceRecipient(
            string odsCode, 
            string name,
            Order order) : this()
        {
            OdsCode = odsCode ?? throw new ArgumentNullException(nameof(odsCode));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }

        public static ServiceRecipient Create(
            string odsCode,
            string name,
            Order order)
        {
            return new ServiceRecipient(odsCode, name, order);
        }

        private bool Equals(ServiceRecipient other)
        {
            return string.Equals(OdsCode, other.OdsCode, StringComparison.OrdinalIgnoreCase)
                   && Equals(Order, other.Order);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ServiceRecipient other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OdsCode, Order);
        }
    }
}
