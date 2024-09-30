using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbContextFactory<OrderManagementDbContext> _dbContextFactory;

    public CustomerRepository(IDbContextFactory<OrderManagementDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Customers
            .FirstOrDefaultAsync(c => c.Email.Address == email, cancellationToken);
    }
}

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken);
}
