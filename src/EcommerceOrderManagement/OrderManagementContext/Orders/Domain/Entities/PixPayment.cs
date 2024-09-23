using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class PixPayment : Entity, IPayment
{
    public PixPayment(string transactionId, Guid orderId)
    {
        TransactionId = transactionId;
        OrderId = orderId;
        Validate();
    }

    private PixPayment() // EF
    { }

    public string TransactionId { get; private set; }
    public Guid OrderId { get; private set; }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(TransactionId))
            throw new ArgumentException("Transaction ID cannot be null or empty.");
        if (OrderId == Guid.Empty)
            throw new ArgumentException("Order ID must be a valid GUID.");
    }
}



