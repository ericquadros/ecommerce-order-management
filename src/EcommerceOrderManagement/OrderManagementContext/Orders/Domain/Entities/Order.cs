using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Events;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderItem> _items;
    private readonly List<IDomainEvent<Order>> _domainEvents;
    private IEnumerable<IDiscountStrategy> _discountStrategies;

    private Order() // EF
    { }

    public Order(Customer customer, List<OrderItem> items, decimal totalAmount)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        _items = items ?? throw new ArgumentNullException(nameof(items));
        TotalAmount = totalAmount;
        Status = OrderStatus.AwaitingProcessing;
        OrderDate=DateTime.Now;
        _domainEvents = new List<IDomainEvent<Order>>();
        AssignOrderIdToItems();
    }

    internal Guid CustomerId { get; }
    public Customer Customer { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items;
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status  { get; private set; }
    public DateTime OrderDate { get; private set; }
    public IEnumerable<IDomainEvent<Order>> Events => _domainEvents;

    public void AddOrderItem(OrderItem item)
    {
        _items.Add(item);
    }
    
    public void ChangeStatus(OrderStatus newStatus)
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

    public void CompleteOrder()
    {
        // Implementar lógica de completar o pedido (ex: atualizar status, emitir eventos)
        if (!_domainEvents.Any(e => e.GetType().Equals(typeof(OrderCompletedEvent))))
            _domainEvents.Add(new OrderCompletedEvent(this));
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
        {
            totalDiscount += strategy.ApplyDiscount(this);
        }

        DiscountAmount = totalDiscount;
        TotalAmount -= DiscountAmount;
    }

    public decimal DiscountAmount { get; set; }
}