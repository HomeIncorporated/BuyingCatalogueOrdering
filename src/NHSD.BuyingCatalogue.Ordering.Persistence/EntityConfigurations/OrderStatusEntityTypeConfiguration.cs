using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.EntityConfigurations
{
    public sealed class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToTable("OrderStatus");

            builder.HasKey(x => x.Id);
        }
    }
}
