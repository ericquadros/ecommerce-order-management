namespace EcommerceOrderManagement.Migrations.Models;

public class OrderModel
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public Guid CustomerId { get; set; }
    public CustomerModel Customer { get; set; }
    public ICollection<OrderItemModel> Items { get; set; }
    public PixPayment? PixPayment { get; set; }
    public CardPaymentModel? CardPayment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}