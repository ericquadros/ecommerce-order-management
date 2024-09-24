using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Events;

public class OrderCompletedEvent : IDomainEvent<Order>
{
    public string EventName { get; set; }
    public Order Object { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderCompletedEvent(Order order)
    {
        EventName = "OrderCompletedWaitingProcessing";
        Object = order;
        OccurredOn = DateTime.Now;
    }
    
    // Constructor to deserialize
    public OrderCompletedEvent()
    {
    }
}