using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.EntityConfigurations
{
    public sealed class ServiceRecipientEntityTypeConfiguration : IEntityTypeConfiguration<ServiceRecipient>
    {
        public void Configure(EntityTypeBuilder<ServiceRecipient> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasKey(x => x.OdsCode);
            
            builder.Property<int>(nameof(Order.OrderId))
                .IsRequired();
        }
    }
}
