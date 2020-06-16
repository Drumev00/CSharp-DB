using Microsoft.EntityFrameworkCore;

using P03_SalesDatabase.Data.Models;

namespace P03_SalesDatabase.Data
{
	public class SalesContext : DbContext
	{
		protected SalesContext()
		{

		}
		public SalesContext(DbContextOptions options) 
			: base(options)
		{

		}

		public DbSet<Product> Products { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Store> Stores { get; set; }
		public DbSet<Sale> Sales { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(Configuring.ConnectionString);
			}

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>(entity =>
			{
				entity.HasKey(p => p.ProductId);

				entity
				.Property(p => p.Name)
				.HasMaxLength(50)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(p => p.Description)
				.HasMaxLength(250)
				.HasDefaultValue("No description")
				.IsRequired(false);

				entity
				.Property(p => p.Quantity)
				.IsRequired();

				entity
				.Property(p => p.Price)
				.IsRequired();

				entity
				.HasMany(p => p.Sales)
				.WithOne(s => s.Product)
				.HasForeignKey(s => s.ProductId);
			});

			modelBuilder.Entity<Customer>(entity =>
			{
				entity.HasKey(c => c.CustomerId);

				entity
				.Property(c => c.Name)
				.HasMaxLength(100)
				.IsRequired()
				.IsUnicode();

				entity
				.Property(c => c.Email)
				.HasMaxLength(80)
				.IsRequired(false)
				.IsUnicode(false);

				entity
				.Property(c => c.CreditCardNumber);

				entity
				.HasMany(c => c.Sales)
				.WithOne(s => s.Customer)
				.HasForeignKey(s => s.CustomerId);
			});

			modelBuilder.Entity<Store>(entity =>
			{
				entity.HasKey(s => s.StoreId);

				entity
				.Property(s => s.Name)
				.HasMaxLength(80)
				.IsRequired()
				.IsUnicode();

				entity
				.HasMany(st => st.Sales)
				.WithOne(s => s.Store)
				.HasForeignKey(s => s.StoreId);
			});

			modelBuilder.Entity<Sale>(entity =>
			{
				entity.HasKey(s => s.SaleId);

				entity
				.Property(s => s.Date)
				.HasColumnType("DATETIME2")
				.HasDefaultValueSql("GETDATE()")
				.IsRequired();
			});
		}
	}
}
