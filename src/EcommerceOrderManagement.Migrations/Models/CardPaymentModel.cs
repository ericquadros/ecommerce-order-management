namespace EcommerceOrderManagement.Migrations.Models;

public class CardPaymentModel
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public string CardHolder { get; set; }
    public string ExpirationDate { get; set; }
    public string Cvv { get; set; }
    public Guid OrderId { get; set; }
    public OrderModel Order { get; set; }
    
    public int Installments { get; set; }
    public bool HasRefund { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}