using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Application.Commands;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;

public class CreateOrderHandler(
    OrderManagementDbContext context,
    OrderRepository orderRepository,
    ProductRepository productRepository,
    CustomerRepository customerRepository,
    IMessageBroker brokerService,
    IEnumerable<IDiscountStrategy> discountStrategies) 
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        // Find or create a new customer
        var customer = await GetOrCreateCustomer(command, cancellationToken);
        
        var productItems = new List<OrderItem>();
        productItems.AddRange(command.Items.Select(i => new OrderItem(new Guid(i.ProductId), i.Quantity, i.Price)));
        var productIdList = productItems.Select(p => p.ProductId);

        var validationResult = await productRepository.ValidateIfExistsProducts(productIdList, cancellationToken);
        if (validationResult.IsFailure)
            return validationResult;

        var order = new Order(customer, productItems, command.TotalAmount);
        
        var paymentResult = ValidateAndCreatePayment(command, order);
        if (paymentResult.IsFailure)
            return Result.Failure(paymentResult.Error);

        order.SetPayment(paymentResult.Value);

        order.AddDiscountStrategies(discountStrategies);
        order.CalculateTotalWithDiscount();
        
        order.CompleteOrder();

        await orderRepository.AddOrderAsync(order, cancellationToken);

        // Publishing domain events
        foreach (var domainEvent in order.Events)
            await brokerService.ProduceMessageAsync(domainEvent);

        return new CreateOrderResponse(
            order.Id.ToString(),
            order.Status.ToString(),
            order.TotalAmount);
    }

    private async Task<Customer> GetOrCreateCustomer(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        return await customerRepository
                   .GetCustomerByEmailAsync(command?.Customer?.Email!, cancellationToken)
                ?? new Customer(command?.Customer?.FirstName!, command?.Customer?.LastName!, new Email(command?.Customer?.Email!), command?.Customer?.Phone!);
    }
    
    private Result<IPayment> ValidateAndCreatePayment(CreateOrderCommand command, Order order)
    {
        if (command.PaymentMethod == OrderPaymentType.Pix.ToString() && command.PixPayment is not null)
        {
            var pixPayment = new PixPayment(
                command.PixPayment.TransactionId,
                command.PixPayment.HasRefund,
                order.Id
            );
            return pixPayment;
        }
        if (command.PaymentMethod == OrderPaymentType.Card.ToString() && command.CardPayment is not null)
        {
            var cardPayment = new CardPayment(
                command.CardPayment.CardNumber,
                command.CardPayment.CardHolder,
                command.CardPayment.ExpirationDate,
                command.CardPayment.Cvv,
                command.CardPayment.Installments,
                command.CardPayment.HasRefund,
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
    