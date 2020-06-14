using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.EntityConfigurations;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<Order> Order { get; set; }

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ILoggerFactory loggerFactory) 
            : base(options)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder?.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AddressEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContactEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceRecipientEntityTypeConfiguration());
        }
    }
}
