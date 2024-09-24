using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.PaymentManagementContext.Payments.Application.Events;

public class OrderProcessingPaymentStatusChangedEvent : IDomainEvent<Order>
{
    public string EventName { get;  set; }
    public Order Object { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderProcessingPaymentStatusChangedEvent(Order order)
    {
        EventName = "OrderPaymentProcessing"; 
        Object = order;
        OccurredOn = DateTime.Now;
    }

    // Constructor to deserialize
    public OrderProcessingPaymentStatusChangedEvent()
    { }
}