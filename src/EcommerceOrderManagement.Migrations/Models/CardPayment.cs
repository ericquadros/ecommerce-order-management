namespace EcommerceOrderManagement.Migrations.Models;

public class CardPayment
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public string CardHolder { get; set; }
    public string ExpirationDate { get; set; }
    public string Cvv { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}