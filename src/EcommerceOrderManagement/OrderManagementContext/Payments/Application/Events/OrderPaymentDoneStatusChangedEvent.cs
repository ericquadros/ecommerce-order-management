using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.Events;

public class OrderPaymentDoneStatusChangedEvent : IDomainEvent<Order>
{
    public string EventName { get; set; }
    public Order Object { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderPaymentDoneStatusChangedEvent(Order order)
    {
        EventName = "OrderPaymentDone"; 
        Object = order;
        OccurredOn = DateTime.Now;
    }
}