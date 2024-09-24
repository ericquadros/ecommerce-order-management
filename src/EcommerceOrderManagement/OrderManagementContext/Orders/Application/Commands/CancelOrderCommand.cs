
using EcommerceOrderManagement.Infrastructure;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Application.Commands;

public class CancelOrderCommand
{
    public Guid OrderId { get; }

    private CancelOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }

    public static Result<CancelOrderCommand> Create(Guid orderId)
    {
        if (orderId == Guid.Empty)
            return Result.Failure("Order ID must be provided.");

        return new CancelOrderCommand(orderId);
    }
}
