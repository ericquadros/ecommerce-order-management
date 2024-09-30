using EcommerceOrderManagement.Migrations.EFMappings;
using EcommerceOrderManagement.Migrations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Migrations.Context;

public class OrderManagementMigrationsDbContext : DbContext
{
    public OrderManagementMigrationsDbContext(DbContextOptions<OrderManagementMigrationsDbContext> options)
        : base(options)
    {}
    
    public DbSet<ProductModel> Products { get; set; }
    public DbSet<ProductCategoryModel> ProductCategories { get; set; }
    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<OrderItemModel> OrderItems { get; set; }
    public DbSet<CustomerModel> Customers { get; set; }
    public DbSet<PixPayment> PixPayments { get; set; }
    public DbSet<CardPaymentModel> CardPayments { get; set; }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.ApplyConfiguration(new ProductMapping());
        modelBuilder.ApplyConfiguration(new ProductCategoryMapping());
        modelBuilder.ApplyConfiguration(new OrderMapping());
        modelBuilder.ApplyConfiguration(new OrderItemMapping());
        modelBuilder.ApplyConfiguration(new CustomerMapping());
        modelBuilder.ApplyConfiguration(new PixPaymentMapping());
        modelBuilder.ApplyConfiguration(new CardPaymentMapping());
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = GetConnectionString(configuration);
                                 
        optionsBuilder.UseSqlServer(connectionString);
    }
    
    private string? GetConnectionString(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("EcommerceOrderManagementDatabase") 
               ?? configuration.GetConnectionString("EcommerceOrderManagementDatabase");
    }
}