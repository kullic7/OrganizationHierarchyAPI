using Microsoft.EntityFrameworkCore;
using OrganizationAPI.Entities;

namespace OrganizationAPI.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<Company> Companies => Set<Company>();
		public DbSet<Employee> Employees => Set<Employee>();
		public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();

		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Company>()
				.HasIndex(c => c.Code)
				.IsUnique();

			modelBuilder.Entity<OrganizationUnit>()
				.HasIndex(o => new { o.CompanyId, o.Code })
				.IsUnique();

			modelBuilder.Entity<Employee>()
				.HasOne(e => e.Company)
				.WithMany(c => c.Employees)
				.HasForeignKey(e => e.CompanyId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<OrganizationUnit>()
				.HasOne(o => o.Company)
				.WithMany(c => c.OrganizationUnits)
				.HasForeignKey(o => o.CompanyId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<OrganizationUnit>()
				.HasOne(o => o.Parent)
				.WithMany(o => o.Children)
				.HasForeignKey(o => o.ParentId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<OrganizationUnit>()
				.HasOne(o => o.Manager)
				.WithMany(e => e.ManagedUnits)
				.HasForeignKey(o => o.ManagerId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}
