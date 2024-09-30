using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;
using EcommerceOrderManagement.Infrastructure;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class PixPayment : Entity, IPayment
{
    private const decimal PERCENTAGE_DISCOUNT = 0.05m; 
    public PixPayment(string transactionId, bool hasRefund, Guid orderId)
    {
        TransactionId = transactionId;
        OrderId = orderId;
        HasRefund = hasRefund;
        Validate();
    }

    private PixPayment() // EF
    { }

    public bool HasRefund { get; private set; }
    public string TransactionId { get; private set; }
    public Guid OrderId { get; private set; }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(TransactionId))
            throw new ArgumentException("Transaction ID cannot be null or empty.");
        if (OrderId == Guid.Empty)
            throw new ArgumentException("Order ID must be a valid GUID.");
    }

    public decimal ApplyDiscount(decimal orderTotal)
    {
        var discount = orderTotal * PERCENTAGE_DISCOUNT;
        if (discount > 0) return discount;
        return 0;
    }
}



