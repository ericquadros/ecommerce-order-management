using EcommerceOrderManagement.Infrastructure.EFContext.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Infrastructure.EFContext;

public class OrderManagementDbContext : DbContext
{
    private static IConfiguration _configuration;
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }
    
    // public DbSet<Product> Products { get; set; }
    // public DbSet<ProductCategory> ProductCategories { get; set; }
    // public DbSet<Order> Orders { get; set; }
    // public DbSet<OrderItem> OrderItems { get; set; }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        // modelBuilder.ApplyConfiguration(new ProductMapping());
        // modelBuilder.ApplyConfiguration(new ProductCategoryMapping());
        // modelBuilder.ApplyConfiguration(new OrderMapping());
        // modelBuilder.ApplyConfiguration(new OrderItemMapping());
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // var configuration = new ConfigurationBuilder()
        //     .SetBasePath(Directory.GetCurrentDirectory())
        //     .AddJsonFile("appsettings.json")
        //     .Build();

        string connectionString = _configuration.GetConnectionString("EcommerceOrderMmanagementDatabase");
                                 
        optionsBuilder.UseSqlServer(connectionString);
    }
    
    public override int SaveChanges()
    {
        try
        {
            foreach (var item in ChangeTracker.Entries().Where(c => c.Entity is IEntity))
            {
                if (item.State == EntityState.Added)
                {
                    item.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
                else if (item.State == EntityState.Modified)
                {
                    item.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}