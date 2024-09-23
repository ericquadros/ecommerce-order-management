using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.Infrastructure.EFContext;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;

// Ajustar essa
public class AjustarPaymentProcessingEventHandler
{
    private readonly OrderManagementDbContext _context;

    public AjustarPaymentProcessingEventHandler(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(OrderCompletedEvent orderCompletedEvent)
    {
        var order = orderCompletedEvent.Object;

        order.SetStatusProcessingPayment();
        
        await _context.AddAsync(order);
        await _context.SaveChangesAsync();
    }
}