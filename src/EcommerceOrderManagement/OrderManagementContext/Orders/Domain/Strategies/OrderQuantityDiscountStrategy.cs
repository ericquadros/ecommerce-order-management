using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;

public class OrderQuantityDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(Order order)
    {
        decimal totalDiscount = 0;

        foreach (var item in order.Items)
        {
            if (item.Quantity > 10)
            {
                if (item.Price <= 10)
                    totalDiscount += 0.50m * item.Quantity;
                else if (item.Price <= 100)
                    totalDiscount += 5m * item.Quantity;
                else if (item.Price <= 1000)
                    totalDiscount += 50m * item.Quantity;
                else if (item.Price <= 10000)
                    totalDiscount += 500m * item.Quantity;
            }
        }

        return totalDiscount;
    }
}
