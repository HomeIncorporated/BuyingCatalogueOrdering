using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.EntityConfigurations
{
    public sealed class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToTable("Address");

            builder.HasKey(o => o.AddressId);

            builder.Property(o => o.AddressId)
                .HasColumnName(nameof(Address.AddressId))
                .IsRequired();

            builder.Property(o => o.Line1)
                .HasColumnName("Line1");

            builder.Property(o => o.Line2)
                .HasColumnName("Line2");

            builder.Property(o => o.Line3)
                .HasColumnName("Line3");

            builder.Property(o => o.Line4)
                .HasColumnName("Line4");

            builder.Property(o => o.Line5)
                .HasColumnName("Line5");
            
            builder.Property(o => o.Town)
                .HasColumnName("Town");

            builder.Property(o => o.County)
                .HasColumnName("County");

            builder.Property(o => o.Postcode)
                .HasColumnName("Postcode");

            builder.Property(o => o.Country)
                .HasColumnName("Country");

            builder.HasOne<Order>()
                .WithOne(o => o.OrganisationAddress)
                .HasPrincipalKey<Address>(address => address.AddressId);

            builder
                .HasOne<Order>()
                .WithOne(o => o.SupplierAddress)
                .HasPrincipalKey<Address>(address => address.AddressId);
        }
    }
}
