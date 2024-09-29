using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Domain.PaymentManagementContext.StockItems.Application.Events;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.Infrastructure.Mail;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.OrderManagementContext.StockItems.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.StockItems.EventHandlers;

public class ProcessPickingItemsOrderEventHandler
{
    private readonly OrderManagementDbContext _context;
    private readonly ILogger<ProcessPickingItemsOrderEventHandler> _logger;
    private readonly InventoryService _inventoryService;
    private readonly CustomerEmailService _emailService;
    private readonly OrderRepository _orderRepository;

    public ProcessPickingItemsOrderEventHandler(
        OrderManagementDbContext context,
        ILogger<ProcessPickingItemsOrderEventHandler> logger,
        InventoryService inventoryService,
        CustomerEmailService emailService,
        OrderRepository orderRepository)
    {
        _context = context;
        _logger = logger;
        _inventoryService = inventoryService;
        _emailService = emailService;
        _orderRepository = orderRepository;
    }

    public async Task<Result<Order>> HandleAsync(OrderPickingItemsStatusChangedEvent orderEvent)
    {
        try
        {
            if (orderEvent?.Object?.Id is null)
                return Result.Failure("The order is not setted in the event.");

            var order = await _orderRepository.GetOrderCompleteAsync(orderEvent?.Object?.Id);

            var allItemsAvailable = true;
            var itemsToDeduct = new List<(Guid ProductId, int Quantity)>();

            foreach (var item in order.Items)
            {
                var isAvailable = await _inventoryService.CheckStockAsync(item.ProductId, item.Quantity);
                if (!isAvailable)
                {
                    allItemsAvailable = false;
                    _logger.LogWarning($"Stock issue for product {item.ProductId}.");
                }
                else
                {
                    // Adiciona o item à lista de deduções de estoque
                    itemsToDeduct.Add((item.ProductId, item.Quantity));
                }
            }

            if (allItemsAvailable)
            {
                // Se todos os itens estão disponíveis, deduz o estoque em memória
                foreach (var item in itemsToDeduct)
                    await _inventoryService.DeductStockAsync(item.ProductId, item.Quantity); // Não salvar ainda

                // Atualiza o pedido como finalizado
                order.FinalizeOrder();
                _logger.LogInformation("All items are in stock, order marked as completed.");
                await NotifyCustomerAboutOrderStatusChange(order); // Notifica o cliente sobre a conclusão do pedido
            }
            else
            {
                // Se algum item não está disponível, o pedido é marcado como aguardando estoque
                order.AwaitingStock();
                _logger.LogInformation("Some items are out of stock, order marked as awaiting stock.");
                await NotifyCustomerAboutOrderStatusChange(order); // Notifica o cliente sobre a mudança de status
            }

            // Atualiza o pedido no contexto e salva
            await _orderRepository.UpdateOrderAsync(order);
            // Enviar e-mails CustomerEmailService

            return order;
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            throw;
        }
    }

    private async Task NotifyCustomerAboutOrderStatusChange(Order order)
    {
        var emailBody = $"Your order with ID {order.Id} has changed status to {order.Status}.";
        var customerEmail = order.Customer.Email; // Acessa o e-mail do cliente
        await _emailService.SendOrderStatusChangeEmail(order.Customer.Email, "Order Status Update", emailBody);
    }
}