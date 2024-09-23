using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.Events;

public class OrderProcessingPaymentStatusChangedEvent : IDomainEvent<Order>
{
    public string EventName { get; private set; }
    public Order Object { get; private set; }
    public DateTime OccurredOn { get; private set; }

    public OrderProcessingPaymentStatusChangedEvent(Order order)
    {
        EventName = "OrderPaymentProcessing"; 
        Object = order;
        OccurredOn = DateTime.Now;
    }
}