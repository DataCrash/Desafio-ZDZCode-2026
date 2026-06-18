using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<DeliveryAddress> DeliveryAddresses => Set<DeliveryAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Price).HasColumnType("decimal(18,2)");

            entity
                .HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(160);
            entity.Property(x => x.Phone).HasMaxLength(30);
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.Property(x => x.CreatedAt).IsRequired();

            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<DeliveryAddress>(entity =>
        {
            entity.ToTable("DeliveryAddresses");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Street).IsRequired().HasMaxLength(180);
            entity.Property(x => x.Number).IsRequired().HasMaxLength(20);
            entity.Property(x => x.District).HasMaxLength(120);
            entity.Property(x => x.City).IsRequired().HasMaxLength(120);
            entity.Property(x => x.State).IsRequired().HasMaxLength(2);
            entity.Property(x => x.ZipCode).IsRequired().HasMaxLength(12);
            entity.Property(x => x.Complement).HasMaxLength(120);

            entity.HasIndex(x => x.CustomerId).IsUnique();

            entity
                .HasOne(x => x.Customer)
                .WithOne(x => x.DeliveryAddress)
                .HasForeignKey<DeliveryAddress>(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
