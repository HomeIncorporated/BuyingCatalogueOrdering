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

            builder.ToTable("ServiceRecipient");

            builder.HasKey(x => new { x.OdsCode, x.OrderId });

            builder.Property(serviceRecipient => serviceRecipient.OdsCode)
                .HasColumnName("OdsCode")
                .IsRequired();

            builder.Property(serviceRecipient => serviceRecipient.OrderId)
                .HasColumnName("OrderId")
                .IsRequired();

            builder.HasOne(serviceRecipient => serviceRecipient.Order)
                .WithMany(o => o.ServiceRecipients);

            builder.Property(serviceRecipient => serviceRecipient.Name)
                .HasColumnName("Name")
                .IsRequired();
        }
    }
}
