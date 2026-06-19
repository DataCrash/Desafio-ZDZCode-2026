using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<DeliveryAddress> DeliveryAddresses => Set<DeliveryAddress>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

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

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(80);
            entity.Property(x => x.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.ToTable("ProductTags");
            entity.HasKey(x => new { x.ProductId, x.TagId });

            entity
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductTags)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.Tag)
                .WithMany(x => x.ProductTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status).IsRequired().HasMaxLength(40);
            entity.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DiscountTotal).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(500);

            entity
                .HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

            entity
                .HasOne(x => x.Order)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(x => x.Product)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
