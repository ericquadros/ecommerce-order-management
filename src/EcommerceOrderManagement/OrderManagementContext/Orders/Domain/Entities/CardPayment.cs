using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class CardPayment : Entity, IPayment
{
    public CardPayment(string cardNumber, string cardHolder, string expirationDate, string cvv, Guid orderId)
    {
        CardNumber = cardNumber;
        CardHolder = cardHolder;
        ExpirationDate = expirationDate;
        CVV = cvv;
        OrderId = orderId;
        Validate();
    }

    private CardPayment() // EF
    { }
    
    public string CardNumber { get; private set; }
    public string CardHolder { get; private set; }
    public string ExpirationDate { get; private set; }
    public string CVV { get; private set; }
    public Guid OrderId { get; private set; }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(CardNumber))
            throw new ArgumentException("Card number cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(CardHolder))
            throw new ArgumentException("Card holder cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(ExpirationDate))
            throw new ArgumentException("Expiration date cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(CVV))
            throw new ArgumentException("CVV cannot be null or empty.");
        if (OrderId == Guid.Empty)
            throw new ArgumentException("Order ID must be a valid GUID.");
    }
}