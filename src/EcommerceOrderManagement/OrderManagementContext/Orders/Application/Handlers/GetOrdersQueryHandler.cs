using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Infrastructure;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;

public class GetOrdersQueryHandler
{
    private readonly OrderRepository _orderRepository;

    public GetOrdersQueryHandler(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<OrderDto>>> Handle(int page, int pageSize, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersAsync(page, pageSize, cancellationToken);
        return orders;
    }
}
