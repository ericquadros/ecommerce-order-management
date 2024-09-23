namespace EcommerceOrderManagement.Migrations.Models;

public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public ICollection<OrderItem> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}