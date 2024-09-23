using EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;
using CreateOrderCommand = EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Application.Commands.CreateOrderCommand;

namespace EcommerceOrderManagement.OrderManagementContext.Endpoints;

using FastEndpoints;

public class CreateOrderEndpoint : Endpoint<CreateOrderRequest>
{
    private readonly CreateOrderHandler _handler;
    public CreateOrderEndpoint(CreateOrderHandler handler)
    {
        _handler = handler;
    }
    
    public override void Configure()
    {
        Post("/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken cancellationToken)
    {
        var commandResult = CreateOrderCommand.Create(req.Customer, req.Items, req.TotalAmount);

        if (commandResult.IsFailure)
        {
            await SendAsync(new { Error = commandResult.Error }, 400, cancellationToken);
            return;
        }

        var result = await _handler.Handle(commandResult.Value!, cancellationToken);
        if (result.IsFailure)
        {
            await SendAsync(new { Error = commandResult.Error }, 400, cancellationToken);
            return;
        }
        
        await SendOkAsync(new { Message = "Order created successfully!", Data = result.Value }, cancellationToken);
    }
}

public record CreateOrderRequest(
    Customer Customer,
    List<OrderItem> Items,
    decimal TotalAmount
    // PaymentDetails Payment 
);

public record Customer(
    string FirstName,                       
    string LastName,                        
    string Email,                           
    string Phone                            
);

public record OrderItem(
    string ProductId,                       
    int Quantity,                           
    decimal Price                       
);

public record PaymentDetails(
    PaymentMethod Method, 
    PixPayment? Pix, 
    CardPayment? CreditCard
);

public enum PaymentMethod
{
    Pix,
    Card
}

public record PixPayment(
    string TransactionId // Exemplo de ID da transação do Pix
);

public record CardPayment(
    string CardNumber,
    string CardHolder,
    string ExpiryDate,
    string Cvv
);