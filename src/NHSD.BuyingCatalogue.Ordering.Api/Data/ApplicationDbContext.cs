using System;
using Microsoft.EntityFrameworkCore;

namespace NHSD.BuyingCatalogue.Ordering.Api.Data
{
	public sealed class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			if (modelBuilder is null)
				throw new ArgumentNullException(nameof(modelBuilder));

			base.OnModelCreating(modelBuilder);
		}
	}
}
