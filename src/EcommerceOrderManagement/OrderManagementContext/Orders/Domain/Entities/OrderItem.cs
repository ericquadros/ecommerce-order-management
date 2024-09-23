using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Migrations.Models;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class OrderItem : Entity
{
    public OrderItem(Guid productId, int quantity, decimal price)
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
    private OrderItem() // EF
    { }

    
    internal Guid ProductId { get; private set; }
    public Product Product { get; private set; }
    internal Guid OrderId { get; private set; }
    // public Order Order { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    public decimal GetTotalPrice() => Quantity * Price;

    public void AssignItemToOrder(Guid orderId) => OrderId = orderId;
    public void AssignItemToProduct(Guid productId) => ProductId = productId;

}