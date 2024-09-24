using EcommerceOrderManagement.OrderManagementContext.Orders.Application.Commands;
using EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;

namespace EcommerceOrderManagement.OrderManagementContext.Endpoints;

using FastEndpoints;

public class CancelOrderEndpoint : Endpoint<CancelOrderRequest>
{
    private readonly CancelOrderHandler _handler;
    
    public CancelOrderEndpoint(CancelOrderHandler handler)
    {
        _handler = handler;
    }
    
    public override void Configure()
    {
        Delete("/orders/{OrderId}/cancel");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancelOrderRequest req, CancellationToken cancellationToken)
    {
        // Create the command to cancel the order
        var commandResult = CancelOrderCommand.Create(req.OrderId);

        if (commandResult.IsFailure)
        {
            await SendAsync(new { Error = commandResult.Error }, 400, cancellationToken);
            return;
        }

        // Handle the cancellation through the handler
        var result = await _handler.Handle(commandResult.Value!, cancellationToken);
        if (result.IsFailure)
        {
            await SendAsync(new { Error = result.Error }, 400, cancellationToken);
            return;
        }
        
        await SendOkAsync(new { Message = "Order cancelled successfully!" }, cancellationToken);
    }
}

public record CancelOrderRequest(Guid OrderId);