namespace EcommerceOrderManagement.Domain.Domain.OrderManagementContext.Endpoints;

using FastEndpoints;

public class CreateOrderEndpoint : Endpoint<CreateOrderRequest>
{
    public override void Configure()
    {
        Post("/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        // Lógica para criar o pedido
        await SendOkAsync(new { Message = "Order created successfully!" }, ct);
    }
}

public record CreateOrderRequest(
    CustomerRequest Customer,
    List<OrderItemRequest> Items,
    decimal TotalAmount
);

public record CustomerRequest(
    string FirstName,                       
    string LastName,                        
    string Email,                           
    string Phone                            
);

public record OrderItemRequest(
    string ProductId,                       
    int Quantity,                           
    decimal UnitPrice                       
);