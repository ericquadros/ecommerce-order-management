using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.OrderManagementContext.Endpoints;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using CardPayment = EcommerceOrderManagement.OrderManagementContext.Endpoints.CardPayment;
using Customer = EcommerceOrderManagement.OrderManagementContext.Endpoints.Customer;
using OrderItem = EcommerceOrderManagement.OrderManagementContext.Endpoints.OrderItem;
using PixPayment = EcommerceOrderManagement.OrderManagementContext.Endpoints.PixPayment;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Application.Commands;

public class CreateOrderCommand
{
    public Customer Customer { get; }
    public List<OrderItem> Items { get; }
    public decimal TotalAmount { get; }
    public string PaymentMethod { get; }
    public PixPayment PixPayment { get; }
    public CardPayment CardPayment { get; }

    private CreateOrderCommand(Customer customer, List<OrderItem> items, decimal totalAmount, string paymentMethod, PixPayment pixPayment, CardPayment cardPayment)
    {
        Customer = customer;
        Items = items;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        PixPayment = pixPayment;
        CardPayment = cardPayment;
    }

    public static Result<CreateOrderCommand> Create(Customer customer, List<OrderItem> items, decimal totalAmount, PaymentDetails paymentDetails)
    {
        var result = Result.Combine(
            ValidateCustomer(customer),
            ValidateItems(items),
            ValidateTotalAmount(totalAmount),
            ValidatePaymentMethod(paymentDetails.Method)
        );

        if (result.IsFailure)
            return Result<CreateOrderCommand>.Failure(result.Error);

        return new CreateOrderCommand(customer, items, totalAmount, paymentDetails.Method, paymentDetails.Pix, paymentDetails.Card);
    }

    private static Result ValidatePaymentMethod(string paymentMethod)
    {
        if (string.IsNullOrEmpty(paymentMethod))
            return Result.Failure("Payment method must be provided.");

        if (paymentMethod != OrderPaymentType.Pix.ToString() && paymentMethod != OrderPaymentType.Card.ToString())
            return Result.Failure("Invalid payment method. Only 'Pix' or 'Card' are accepted.");

        return Result.Success();
    }

    private static Result ValidateCustomer(Customer customer)
    {
        if (string.IsNullOrEmpty(customer.FirstName))
            return Result.Failure("First name must be provided.");
        if (string.IsNullOrEmpty(customer.LastName))
            return Result.Failure("Last name must be provided.");
        if (string.IsNullOrEmpty(customer.Email))
            return Result.Failure("Email must be provided.");
        if (string.IsNullOrEmpty(customer.Phone))
            return Result.Failure("Phone must be provided.");

        return Result.Success();
    }

    private static Result ValidateItems(List<OrderItem> items)
    {
        if (items == null || !items.Any())
            return Result.Failure("At least one item must be provided.");

        var itemsWithoutPrice = items.Any(i => i.Price <= 0);
        if (itemsWithoutPrice)
            return Result.Failure("Some item does not have price.");
        
        var itemsWithoutQuantity = items.Any(i => i.Quantity < 1);
        if (itemsWithoutQuantity)
            return Result.Failure("Some item does not have price.");

        return Result.Success();
    }

    private static Result ValidateTotalAmount(decimal totalAmount)
    {
        if (totalAmount <= 0)
            return Result.Failure("Total amount must be greater than zero.");

        return Result.Success();
    }
}
