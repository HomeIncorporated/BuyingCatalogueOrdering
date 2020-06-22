using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Ordering.Application.Services;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.EntityConfigurations;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        private readonly IIdentityService _identityService;
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<Order> Orders { get; set; }

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IIdentityService identityService,
            ILoggerFactory loggerFactory) 
            : base(options)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder?
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging();
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

        public override int SaveChanges()
        {
            BeforeSave();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            BeforeSave();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void BeforeSave()
        {
            foreach (var entity in ChangeTracker.Entries<Order>())
            {
                var currentValues = entity.CurrentValues;

                switch (entity.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        currentValues[nameof(Domain.Order.LastUpdatedBy)] = _identityService.GetUserIdentity();
                        currentValues[nameof(Domain.Order.LastUpdatedByName)] = _identityService.GetUserName();
                        break;
                }
            }
        }
    }
}
