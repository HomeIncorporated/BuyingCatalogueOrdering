using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.EntityConfigurations
{
    public sealed class ContactEntityTypeConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToTable("Contact");

            builder.HasKey(o => o.ContactId);

            builder.Property(o => o.ContactId)
                .HasColumnName(nameof(Contact.ContactId))
                .IsRequired();

            builder.Property(o => o.FirstName)
                .HasColumnName("FirstName");

            builder.Property(o => o.LastName)
                .HasColumnName("LastName");

            builder.Property(o => o.Email)
                .HasColumnName("Email");

            builder.Property(o => o.Phone)
                .HasColumnName("Phone");
        }
    }
}
