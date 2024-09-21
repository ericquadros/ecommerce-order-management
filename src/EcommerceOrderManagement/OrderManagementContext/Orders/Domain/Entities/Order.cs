using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Events;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderItem> _items;
    private readonly List<IDomainEvent<Order>> _domainEvents;

    private Order() // EF
    { }

    public Order(Customer customer, List<OrderItem> items, decimal totalAmount)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        _items = items ?? throw new ArgumentNullException(nameof(items));
        TotalAmount = totalAmount;
        Status = EOrderStatus.AwaitingProcessing;
        OrderDate=DateTime.Now;
        _domainEvents = new List<IDomainEvent<Order>>();
        AssignOrderIdToItems();
    }

    internal Guid CustomerId { get; }
    public Customer Customer { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items;
    public decimal TotalAmount { get; private set; }
    public EOrderStatus Status  { get; private set; }
    public DateTime OrderDate { get; private set; }
    public IEnumerable<IDomainEvent<Order>> Events => _domainEvents;

    public void AddOrderItem(OrderItem item)
    {
        _items.Add(item);
    }
    
    public void ChangeStatus(EOrderStatus newStatus)
    {
        Status = newStatus;
        _domainEvents.Add(new OrderStatusChangedDomainEvent(this));
    }

    // Precisei fazer isto pois não vinculava automático o id da order no order item
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
        _domainEvents.Add(new OrderCompletedEvent(this));
    }
}