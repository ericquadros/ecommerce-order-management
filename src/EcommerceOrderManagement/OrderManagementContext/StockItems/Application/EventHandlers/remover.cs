using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Domain.OrderManagementContext.StockItems.Application.EventHandlers;
//
// public class ProcessPickingItemsOrderEventHandler
// {
//     private readonly OrderManagementDbContext _context;
//     private readonly ILogger<ProcessPickingItemsOrderEventHandler> _logger;
//     private readonly IInventoryService _inventoryService;
//     private readonly IEmailService _emailService;
//
//     public ProcessPickingItemsOrderEventHandler(
//         OrderManagementDbContext context,
//         ILogger<ProcessPickingItemsOrderEventHandler> logger,
//         IInventoryService inventoryService,
//         IEmailService emailService)
//     {
//         _context = context;
//         _logger = logger;
//         _inventoryService = inventoryService;
//         _emailService = emailService;
//     }
//
//     public async Task<Result<Order>> HandleAsync(OrderPickingItemsEvent orderEvent)
//     {
//         var order = orderEvent.Object;
//         var allItemsAvailable = true;
//
//         foreach (var item in order.Items)
//         {
//             var isAvailable = await _inventoryService.CheckStockAsync(item.ProductId, item.Quantity);
//             if (!isAvailable)
//             {
//                 allItemsAvailable = false;
//                 await NotifySalesAboutStockIssue(item, order);
//                 _logger.LogWarning($"Stock issue for product {item.ProductId}, notifying sales.");
//             }
//             else
//             {
//                 await _inventoryService.DeductStockAsync(item.ProductId, item.Quantity);
//             }
//         }
//
//         if (allItemsAvailable)
//         {
//             order.SetStatusCompleted();
//             _logger.LogInformation("All items are in stock, order marked as completed.");
//         }
//         else
//         {
//             order.SetStatusAwaitingStock();
//             _logger.LogInformation("Some items are out of stock, order marked as awaiting stock.");
//         }
//
//         _context.Orders.Update(order);
//         await _context.SaveChangesAsync();
//
//         return order;
//     }
//
//     private async Task NotifySalesAboutStockIssue(OrderItem item, Order order)
//     {
//         var emailBody = $"The product with ID {item.ProductId} is out of stock for order {order.Id}.";
//         await _emailService.SendEmailAsync("sales@company.com", "Stock Issue Alert", emailBody);
//     }
// }