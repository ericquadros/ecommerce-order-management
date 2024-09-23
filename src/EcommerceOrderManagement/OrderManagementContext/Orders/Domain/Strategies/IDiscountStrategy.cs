using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;

public interface IDiscountStrategy
{
    decimal ApplyDiscount(Order order);
}