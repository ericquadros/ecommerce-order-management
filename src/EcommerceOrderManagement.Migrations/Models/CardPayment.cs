namespace EcommerceOrderManagement.Migrations.Models;

public class CardPayment
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public string CardHolder { get; set; }
    public string ExpiryDate { get; set; }
    public string Cvv { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}