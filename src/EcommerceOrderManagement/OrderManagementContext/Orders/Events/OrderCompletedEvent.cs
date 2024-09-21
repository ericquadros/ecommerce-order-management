using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;

public class OrderCompletedEvent : IDomainEvent<Order>
{
    public string EventName { get; private set; }
    public Order Object { get; private set; }
    public DateTime OccurredOn { get; private set; }

    public OrderCompletedEvent(Order order)
    {
        EventName = "OrderCompleted";
        Object = order;
        OccurredOn = DateTime.UtcNow;
    }
}