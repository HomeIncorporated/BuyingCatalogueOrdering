using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Domain.Common;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.EntityConfigurations
{
    public sealed class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToTable("Order");

            builder.HasKey(o => o.OrderId);

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasConversion(
                    description => description.Value,
                    data => OrderDescription.Create(data).Value);

            builder.Property(o => o.OrganisationId)
                .HasColumnName("OrganisationId")
                .IsRequired();

            builder.Property(o => o.OrganisationName)
                .HasColumnName("OrganisationName");

            builder.Property(o => o.OrganisationOdsCode)
                .HasColumnName("OrganisationOdsCode");

            builder
                .HasMany(x => x.ServiceRecipients)
                .WithOne(s => s.Order);

            builder.HasOne(o => o.OrganisationAddress)
                .WithOne()
                .HasForeignKey<Order>(order => order.OrganisationAddressId);

            builder.HasOne(o => o.OrganisationContact)
                .WithOne()
                .HasForeignKey<Order>(o => o.OrganisationContactId);

            builder
                .Property(x => x.OrderStatus)
                .HasConversion(orderStatus => orderStatus.Id, data => Enumeration.FromValue<OrderStatus>(data))
                .HasColumnName("OrderStatusId");

            builder.Property(o => o.ServiceRecipientsViewed)
                .HasColumnName("ServiceRecipientsViewed");

            builder.Property(o => o.SupplierId)
                .HasColumnName("SupplierId");

            builder.Property(o => o.SupplierName)
                .HasColumnName("SupplierName");

            builder.HasOne(o => o.SupplierAddress)
                .WithOne()
                .HasForeignKey<Order>(order => order.SupplierAddressId);

            builder.HasOne(o => o.SupplierContact)
                .WithOne()
                .HasForeignKey<Order>(o => o.SupplierContactId);

            builder.Property(o => o.CatalogueSolutionsViewed)
                .HasColumnName("CatalogueSolutionsViewed");

            builder.Property(o => o.CommencementDate)
                .HasColumnName("CommencementDate");

            builder.Property(o => o.Created)
                .HasColumnName("Created");

            builder.Property(o => o.LastUpdated)
                .HasColumnName("LastUpdated")
                .IsRequired();

            builder.Property(o => o.LastUpdatedBy)
                .HasColumnName("LastUpdatedBy")
                .IsRequired();

            builder.Property(o => o.LastUpdatedByName)
                .HasColumnName("LastUpdatedByName")
                .IsRequired();

            //    OrganisationBillingAddressId INT NULL,
        }
    }
}
