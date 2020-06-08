using NHSD.BuyingCatalogue.Ordering.Domain.Common;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Submitted = new OrderStatus(1, nameof(Submitted));
        public static readonly OrderStatus Unsubmitted = new OrderStatus(2, nameof(Unsubmitted));

        private OrderStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}
