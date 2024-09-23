using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.Infrastructure.Interfaces;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Application.Commands;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;

public class CreateOrderHandler(
    OrderManagementDbContext context,
    IMessageBroker brokerService,
    IEnumerable<IDiscountStrategy> discountStrategies) : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        // Find or create a new customer
        var customer = await context.Customers
                           .FirstOrDefaultAsync(c => c.Email.Address == command.Customer.Email, cancellationToken)
                       ?? new Customer(command.Customer.FirstName, command.Customer.LastName, new Email(command.Customer.Email), command.Customer.Phone);
        
        var orderItems = new List<OrderItem>();
        orderItems.AddRange(command.Items.Select(i => new OrderItem(new Guid(i.ProductId), i.Quantity, i.Price)));
        
        var validationResult = await ValidateIfExistsProducts(orderItems, context, cancellationToken);
        if (validationResult.IsFailure)
            return validationResult;

        var order = new Order(customer, orderItems, command.TotalAmount);
        
        // Validar e associar o pagamento usando o método privado
        var paymentResult = ValidateAndCreatePayment(command, order);
        if (paymentResult.IsFailure)
            return Result.Failure(paymentResult.Error);

        order.SetPayment(paymentResult.Value);

        order.AddDiscountStrategies(discountStrategies);
        order.CalculateTotalWithDiscount();
        
        order.CompleteOrder();
        
        await context.Orders.AddAsync(order, cancellationToken);
        var entitiesAdded = await context.SaveEntitiesAsync(cancellationToken);

        // Publishing domain events
        foreach (var domainEvent in order.Events)
            await brokerService.ProduceMessageAsync(domainEvent);

        return new CreateOrderResponse(
            order.Id.ToString(),
            order.Status.ToString(),
            order.TotalAmount);
    }
    
    private async Task<Result> ValidateIfExistsProducts(IEnumerable<OrderItem> items, OrderManagementDbContext context, CancellationToken cancellationToken)
    {
        var productIds = items.Select(i => i.ProductId).ToList();
        var existingProducts = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (existingProducts.Count != productIds.Count)
            return Result.Failure("One or more products does not exists.");
        
        return Result.Success();
    }
    
    private Result<IPayment> ValidateAndCreatePayment(CreateOrderCommand command, Order order)
    {
        if (command.PaymentMethod == OrderPaymentType.Pix.ToString() && command.PixPayment != null)
        {
            var pixPayment = new PixPayment(
                command.PixPayment.TransactionId,
                order.Id
            );
            return pixPayment;
        }
        if (command.PaymentMethod == OrderPaymentType.Card.ToString() && command.CardPayment != null)
        {
            // Criar a instância do pagamento Cartão
            var cardPayment = new CardPayment(
                command.CardPayment.CardNumber,
                command.CardPayment.CardHolder,
                command.CardPayment.ExpirationDate,
                command.CardPayment.Cvv,
                order.Id
            );
            return cardPayment;
        }
        
        return Result.Failure("Invalid payment method.");
    }
}

public interface IRequestHandler<T, T1>
{
}

public record CreateOrderResponse(
    string OrderId,
    string Status,
    decimal TotalAmount);
