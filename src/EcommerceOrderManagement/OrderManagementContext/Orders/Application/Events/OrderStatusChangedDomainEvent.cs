using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Events;

public class OrderStatusChangedDomainEvent : IDomainEvent<Order>
{
    public string EventName { get; set; }
    public Order Object { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderStatusChangedDomainEvent(Order order)
    {
        EventName = "OrderStatusChanged";
        Object = order;
        OccurredOn = DateTime.UtcNow;
    }
}