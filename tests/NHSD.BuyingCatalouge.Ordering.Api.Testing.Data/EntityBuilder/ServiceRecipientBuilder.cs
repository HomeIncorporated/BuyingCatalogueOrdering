using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.EntityBuilder
{
    public sealed class ServiceRecipientBuilder
    {
        private string _odsCode;
        private string _name;
        private int _orderId;

        private ServiceRecipientBuilder()
        {
        }

        public static ServiceRecipientBuilder Create()
        {
            return new ServiceRecipientBuilder();
        }

        public ServiceRecipientBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        public ServiceRecipientBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ServiceRecipientBuilder WithOrderId(int orderId)
        {
            _orderId = orderId;
            return this;
        }

        public ServiceRecipientEntity Build()
        {
            return new ServiceRecipientEntity
            {
                OdsCode = _odsCode,
                Name = _name,
                OrderId = _orderId
            };
        }
    }
}
