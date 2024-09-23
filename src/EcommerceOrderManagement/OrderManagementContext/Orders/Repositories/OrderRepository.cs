using EcommerceOrderManagement.Infrastructure.EFContext;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderManagementDbContext _context;

    public OrderRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Orders
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