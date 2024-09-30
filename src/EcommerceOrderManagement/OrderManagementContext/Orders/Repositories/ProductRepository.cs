using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDbContextFactory<OrderManagementDbContext> _dbContextFactory;

    public ProductRepository(IDbContextFactory<OrderManagementDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<Result> ValidateIfExistsProducts(IEnumerable<Guid> productIds, CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var existingProducts = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (existingProducts.Count != productIds.Count())
            return Result.Failure("One or more products does not exist.");

        return Result.Success();
    }
}

public interface IProductRepository
{
    Task<Result> ValidateIfExistsProducts(IEnumerable<Guid> productIds, CancellationToken cancellationToken);
}
