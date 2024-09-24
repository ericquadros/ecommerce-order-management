using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.Infrastructure;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class CardPayment : Entity, IPayment
{
    private const int MAX_INSTALLMENTS = 12;
    public CardPayment(string cardNumber, string cardHolder, string expirationDate, string cvv, int installments, Guid orderId)
    {
        CardNumber = cardNumber;
        CardHolder = cardHolder;
        ExpirationDate = expirationDate;
        Cvv = cvv;
        OrderId = orderId;
        Installments = installments;
        Validate();
    }

    private CardPayment() // EF
    { }
    
    public string CardNumber { get; private set; }
    public string CardHolder { get; private set; }
    public string ExpirationDate { get; private set; }
    public string Cvv { get; private set; }
    public Guid OrderId { get; private set; }
    public int Installments { get; private set; }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(CardNumber))
            throw new ArgumentException("Card number cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(CardHolder))
            throw new ArgumentException("Card holder cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(ExpirationDate))
            throw new ArgumentException("Expiration date cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(Cvv))
            throw new ArgumentException("CVV cannot be null or empty.");
        if (OrderId == Guid.Empty)
            throw new ArgumentException("Order ID must be a valid GUID.");
        if (Installments > MAX_INSTALLMENTS)
            throw new ArgumentException($"Payment with credit cannot exceed {MAX_INSTALLMENTS} installments.");
    }
}