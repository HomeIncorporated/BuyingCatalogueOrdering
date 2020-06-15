using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class ServiceRecipientBuilder
    {
        private string _odsCode;
        private readonly string _name;
        private string _orderId;

        private ServiceRecipientBuilder()
        {
            _name = "Some name";
        }

        internal static ServiceRecipientBuilder Create() 
            => new ServiceRecipientBuilder();

        internal ServiceRecipientBuilder WithOdsCode(string odsCode)
        {
            _odsCode = odsCode;
            return this;
        }

        internal ServiceRecipientBuilder WithOrderId(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        internal ServiceRecipient Build()
        {
            var serviceRecipient = new ServiceRecipient(_odsCode, _name);

            serviceRecipient.SetOrder(OrderBuilder.Create().WithOrderId(_orderId).Build());

            return serviceRecipient;
        }

    }
}
