using System.Globalization;
using EcommerceOrderManagement.Migrations.EFMappings;
using EcommerceOrderManagement.Migrations.Models;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Infrastructure.EFContext;

public class OrderManagementDbContextFactory : IDbContextFactory, IDbContextFactory<OrderManagementDbContext>
{
    private readonly DbContextOptions<OrderManagementDbContext> _options;

    public OrderManagementDbContextFactory(
        DbContextOptions<OrderManagementDbContext> options)
    {
        _options = options;
    }

    public OrderManagementDbContext CreateDbContext()
    {
        return new OrderManagementDbContext(_options);
    }
}

public interface IDbContextFactory
{
    OrderManagementDbContext CreateDbContext();
}
