using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.StockItems.Application.Events;

public class OrderPickingItemsStatusChangedEvent : IDomainEvent<Order>
{
    public string EventName { get; set; }
    public Order Object { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderPickingItemsStatusChangedEvent(Order order)
    {
        EventName = "PickingOrder"; 
        Object = order;
        OccurredOn = DateTime.Now;
    }
}