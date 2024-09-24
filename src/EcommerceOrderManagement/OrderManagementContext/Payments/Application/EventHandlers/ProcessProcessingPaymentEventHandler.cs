using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.Events;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;

public class ProcessProcessingPaymentEventHandler
{
    private readonly OrderManagementDbContext _context;

    public ProcessProcessingPaymentEventHandler(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Order>> HandleAsync(OrderProcessingPaymentStatusChangedEvent orderEvent)
    {
        var order = orderEvent.Object;
        //
        // order.SetStatusProcessingPayment();
        //
        // await _context.AddAsync(order);
        // await _context.SaveChangesAsync();
    }
}