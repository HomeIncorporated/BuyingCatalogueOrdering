using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Summary
{
    public sealed class SectionModel
    {
        public static SectionModel Description => new SectionModel("description", "complete");
        public static SectionModel OrderingParty => new SectionModel("ordering-party");
        public static SectionModel Supplier => new SectionModel("supplier");
        public static SectionModel CommencementDate => new SectionModel("commencement-date");
        public static SectionModel AssociatedServices => new SectionModel("associated-services");
        public static SectionModel ServiceRecipients => new SectionModel("service-recipients");
        public static SectionModel CatalogueSolutions => new SectionModel("catalogue-solutions");
        public static SectionModel AdditionalServices => new SectionModel("additional-services");
        public static SectionModel FundingSource => new SectionModel("funding-source");

        public string Id { get; }

        public string Status { get; private set; }

        public int? Count { get; private set; }

        private SectionModel(string id, string status = "incomplete")
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public SectionModel WithStatus(string status)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            return this;
        }

        public SectionModel WithCount(int count)
        {
            Count = count;
            return this;
        }
    }
}
