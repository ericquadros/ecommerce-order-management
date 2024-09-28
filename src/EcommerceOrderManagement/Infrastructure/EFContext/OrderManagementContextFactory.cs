using System.Globalization;
using EcommerceOrderManagement.Migrations.EFMappings;
using EcommerceOrderManagement.Migrations.Models;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Infrastructure.EFContext;

public class OrderManagementDbContextFactory : IDbContextFactory
{
    private readonly DbContextOptions<OrderManagementDbContext> _options;
    private readonly IConfiguration _configuration;

    public OrderManagementDbContextFactory(
        DbContextOptions<OrderManagementDbContext> options,
        IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;
    }

    public OrderManagementDbContext CreateDbContext()
    {
        return new OrderManagementDbContext(_options, _configuration);
    }
}

public interface IDbContextFactory
{
    OrderManagementDbContext CreateDbContext();
}
