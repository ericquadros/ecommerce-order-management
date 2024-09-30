using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;

public class OrderSeasonalDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(Order order)
    {
        var septemberMonth = 9;
        var octoberMonth = 10;
        // Verify if we stay in september or october month
        if (order.OrderDate.Month == septemberMonth || order.OrderDate.Month == octoberMonth)
        {
            return order.TotalAmount * 0.05m; // 5% of discount
        }

        return 0;
    }
}