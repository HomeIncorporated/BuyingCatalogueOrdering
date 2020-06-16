using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientBuilder
    {
        private string _odsCode;
        private readonly string _name;
        private int _orderId;

        private ServiceRecipientBuilder()
        {
            _name = "Some name";
        }

        internal static ServiceRecipientBuilder Create() => new ServiceRecipientBuilder();

        internal ServiceRecipientBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        internal ServiceRecipientBuilder WithOrderId(int orderId)
        {
            _orderId = orderId;
            return this;
        }

        internal ServiceRecipient Build() => 
            new ServiceRecipient
            {
                OdsCode = _odsCode,
                Name = _name,
                Order = OrderBuilder
                    .Create()
                    .WithOrderId(_orderId)
                    .Build()
            };
    }
}
