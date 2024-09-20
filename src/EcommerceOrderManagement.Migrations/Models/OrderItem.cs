namespace EcommerceOrderManagement.Migrations.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    
    public Product Product { get; set; } 
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}