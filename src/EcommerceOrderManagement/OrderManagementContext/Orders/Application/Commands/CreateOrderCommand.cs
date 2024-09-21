using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.OrderManagementContext.Endpoints;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Application.Commands;

public class CreateOrderCommand
{
    public Customer Customer { get; }
    public List<OrderItem> Items { get; }
    public decimal TotalAmount { get; }

    private CreateOrderCommand(Customer customer, List<OrderItem> items, decimal totalAmount)
    {
        Customer = customer;
        Items = items;
        TotalAmount = totalAmount;
    }

    public static Result<CreateOrderCommand> Create(Customer customer, List<OrderItem> items, decimal totalAmount)
    {
        var result = Result.Combine(
            ValidateCustomer(customer),
            ValidateItems(items),
            ValidateTotalAmount(totalAmount)
        );

        if (result.IsFailure)
            return Result<CreateOrderCommand>.Failure(result.Error);

        return new CreateOrderCommand(customer, items, totalAmount);
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
