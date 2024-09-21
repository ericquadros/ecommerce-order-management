using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Events;

public class OrderStatusChangedDomainEvent : IDomainEvent<Order>
{
    public string EventName { get; private set; }
    public Order Object { get; private set; }
    public DateTime OccurredOn { get; private set; }

    public OrderStatusChangedDomainEvent(Order order)
    {
        EventName = "OrderStatusChanged";
        Object = order;
        OccurredOn = DateTime.UtcNow;
    }
}