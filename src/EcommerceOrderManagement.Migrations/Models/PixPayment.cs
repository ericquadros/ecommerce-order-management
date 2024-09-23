namespace EcommerceOrderManagement.Migrations.Models;

public class PixPayment
{
    public Guid Id { get; set; }
    public string TransactionId { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}