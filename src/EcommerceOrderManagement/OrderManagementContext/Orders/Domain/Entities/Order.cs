using System.Text.Json.Serialization;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.Events;
using EcommerceOrderManagement.Domain.PaymentManagementContext.StockItems.Application.Events;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.PaymentManagementContext.Payments.Application.Events;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderItem> _items;
    [JsonIgnore]
    private readonly List<IDomainEvent<Order>> _domainEvents;
    private IEnumerable<IDiscountStrategy> _discountStrategies;

    private Order() // EF
    { }

    public Order(Customer customer, List<OrderItem> items, decimal totalAmount,  PixPayment? pixPayment = null, CardPayment? cardPayment = null)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        _items = items ?? throw new ArgumentNullException(nameof(items));
        TotalAmount = totalAmount;
        Status = OrderStatus.AwaitingProcessing;
        OrderDate=DateTime.Now;
        _domainEvents = new List<IDomainEvent<Order>>();
        PixPayment = pixPayment;
        CardPayment = cardPayment;
        AssignOrderIdToItems();
    }

    internal Guid CustomerId { get; }
    public Customer Customer { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items;
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status  { get; private set; }
    public DateTime OrderDate { get; private set; }
    
    public PixPayment? PixPayment { get; private set; }
    public CardPayment? CardPayment { get; private set; }
    
    public IEnumerable<IDomainEvent<Order>> Events => _domainEvents;

    private void AddOrderItem(OrderItem item)
    {
        _items.Add(item);
    }
    
    private void ChangeStatus(OrderStatus newStatus)
    {
        Status = newStatus;
        _domainEvents.Add(new OrderStatusChangedDomainEvent(this));
    }

    // TODO: Resolve. I needed to do this because the order ID was not automatically linked to the order item.
    public void AssignOrderIdToItems()
    {
        foreach (var item in _items)
        {
            item.AssignItemToOrder(Id);
        }
    }

    public Result CompleteOrder()
    {
        if (PixPayment is null && CardPayment is null)
            return Result.Failure("You need to set the payment.");
        
        // Implementar lógica de completar o pedido (ex: atualizar status, emitir eventos)
        if (!_domainEvents.Any(e => e.GetType().Equals(typeof(OrderCompletedEvent))))
            _domainEvents.Add(new OrderCompletedEvent(this));

        return Result.Success();
    }
    
    public Result FinalizeOrder()
    {
        if (Status != OrderStatus.PickingOrder)
            return Result.Failure("The status should be PickingOrder");
        
        Status = OrderStatus.Completed;

        return Result.Success();
    }
    
    public Result SetStatusProcessingPayment()
    {
        if (Status != OrderStatus.AwaitingProcessing)
            return Result.Failure("The status should be AwaitingProcessing");
        
        if (PixPayment is null && CardPayment is null)
            return Result.Failure("You need to set the payment.");

        Status = OrderStatus.ProcessingPayment;
        
        // if (!_domainEvents.Any(e => e.GetType().Equals(typeof(OrderProcessingPaymentStatusChangedEvent))))
        _domainEvents.Add(new OrderProcessingPaymentStatusChangedEvent(this));

        return Result.Success();
    }

    public Result SetStatusPickingOrder()
    {
        // StringToDateOnlyConverter: REMOVE
        // if (Status != OrderStatus.PaymentCompleted)
        //     return Result.Failure("The status should be PaymentCompleted");

        Status = OrderStatus.PickingOrder;
        _domainEvents.Add(new OrderPickingItemsStatusChangedEvent(this));
        
        return Result.Success();
    }
    
    public Result SetStatusAwaitingStock()
    {
        if (Status != OrderStatus.PickingOrder) 
            return Result.Failure("The status should be PickingOrder");
        
        Status = OrderStatus.WaitingForStock;

        return Result.Success();
    }
    
    public Result CancelOrder()
    {
        if (Status == OrderStatus.AwaitingProcessing)
        {
            Status = OrderStatus.Cancelled;
            return Result.Success();
        }

        return Result.Failure("Only orders awaiting processing can be cancelled.");
        // AddDomainEvent(new OrderCancelledEvent(this));
    }

    public void AddDiscountStrategies(IEnumerable<IDiscountStrategy> discountStrategies)
    {
        _discountStrategies = discountStrategies;
    }

    public void CalculateTotalWithDiscount()
    {
        decimal totalDiscount = 0;

        // Apply all discount strategies
        foreach (var strategy in _discountStrategies)
            totalDiscount += strategy.ApplyDiscount(this);

        if (PixPayment is not null)
            totalDiscount += PixPayment.ApplyDiscount(TotalAmount);

        DiscountAmount = totalDiscount;
        TotalAmount -= DiscountAmount;
    }

    public decimal DiscountAmount { get; set; }

    public void SetPayment(IPayment payment)
    {
        if (payment is PixPayment)
            PixPayment = (PixPayment)payment;
        else
            CardPayment = (CardPayment)payment;
    }

    public Result PaymentCompleted()
    {
        Status = OrderStatus.PaymentCompleted;
        
        if (!_domainEvents.Any(e => e.GetType().Equals(typeof(OrderPaymentDoneStatusChangedEvent))))
            _domainEvents.Add(new OrderPaymentDoneStatusChangedEvent(this));

        return Result.Success();
    }
}

public enum OrderPaymentType
{
    Pix,
    Card
} 