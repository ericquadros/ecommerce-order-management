using System.Globalization;
using EcommerceOrderManagement.Migrations.EFMappings;
using EcommerceOrderManagement.Migrations.Models;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Infrastructure.EFContext;

public class OrderManagementDbContext : DbContext, IUnitOfWork
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<CardPayment> CardPayments { get; set; }
    public DbSet<PixPayment> PixPayments { get; set; }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.ApplyConfiguration(new OrderMapping());
        modelBuilder.ApplyConfiguration(new CustomerMapping());
        modelBuilder.ApplyConfiguration(new OrderItemMapping());
        modelBuilder.ApplyConfiguration(new ProductCategoryMapping());
        modelBuilder.ApplyConfiguration(new ProductMapping());
        modelBuilder.ApplyConfiguration(new PixPaymentMapping());
        modelBuilder.ApplyConfiguration(new CardPaymentMapping());
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
                                 
        optionsBuilder
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
    }
    
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var item in ChangeTracker.Entries().Where(c => c.Entity is IEntity))
            {
                if (item.State == EntityState.Added)
                {
                    item.Property("CreatedAt").CurrentValue = DateTime.Now;
                    item.Property("UpdatedAt").CurrentValue = DateTime.Now;
                }
                else if (item.State == EntityState.Modified)
                    item.Property("UpdatedAt").CurrentValue = DateTime.Now;
            }
        
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return result > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public interface IUnitOfWork
{
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}