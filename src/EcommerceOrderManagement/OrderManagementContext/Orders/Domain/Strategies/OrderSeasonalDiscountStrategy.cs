using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;

public class OrderSeasonalDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(Order order)
    {
        var septemberMonth = 9;
        // Verify if we stay in september month
        if (order.OrderDate.Month == septemberMonth)
        {
            return order.TotalAmount * 0.05m; // 5% of discount
        }

        return 0;
    }
}