using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IDbContextFactory<OrderManagementDbContext> _dbContextFactory;

    public OrderRepository(IDbContextFactory<OrderManagementDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<Order?> GetOrderCompleteAsync(Guid? orderId)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Orders
            .Include(o => o.Items)
            .Include(o => o.Customer)
            .Include(o => o.PixPayment)
            .Include(o => o.CardPayment)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<List<OrderDto>> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Orders
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(order => new OrderDto
            {
                OrderId = order.Id,
                CustomerName = $"{order.Customer.FirstName} {order.Customer.LastName}",
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
    
    public async Task UpdateOrderAsync(Order order)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Orders.Update(order);
        await context.SaveChangesAsync();
    }
}

public interface IOrderRepository
{
    Task<List<OrderDto>> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken);
}

public class OrderDto
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}