using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Application.Commands;
using EcommerceOrderManagement.Infrastructure.EFContext;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;

public class CancelOrderHandler
{
    private readonly OrderManagementDbContext _context;

    public CancelOrderHandler(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == command.OrderId);
        if (order == null)
            return Result.Failure("Order not found.");

        var result = order.CancelOrder();
        if (result.IsFailure)
            return result;

        await _context.SaveChangesAsync();
        
        return Result.Success();
    }
}
