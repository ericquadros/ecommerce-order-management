using EcommerceOrderManagement.Infrastructure.EFContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.OrderManagementContext.StockItems.Domain.Services;

public class InventoryService
{
    private readonly OrderManagementDbContext _context;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(OrderManagementDbContext context, ILogger<InventoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> CheckStockAsync(Guid productId, int quantity)
    {
        var stockItem = await _context.Products
            .FirstOrDefaultAsync(s => s.Id == productId);
        if (stockItem == null)
        {
            _logger.LogWarning($"Produto {productId} não encontrado no estoque.");
            return false;
        }

        if (stockItem.StockQuantity >= quantity)
        {
            _logger.LogInformation($"Produto {productId} tem estoque suficiente.");
            return true;
        }

        _logger.LogWarning($"Produto {productId} não tem estoque suficiente. Disponível: {stockItem.StockQuantity}, Necessário: {quantity}.");
        return false;
    }

    public async Task DeductStockAsync(Guid productId, int quantity, bool saveChanges = false)
    {
        var stockItem = await _context.Products
            .FirstOrDefaultAsync(s => s.Id == productId);

        if (stockItem == null)
        {
            _logger.LogError($"Tentativa de deduzir estoque de produto não encontrado. ProdutoId: {productId}");
            throw new InvalidOperationException($"Produto {productId} não encontrado no estoque.");
        }

        stockItem.StockQuantity -= quantity;

        if (stockItem.StockQuantity < 0)
        {
            _logger.LogWarning($"Produto {productId} ficou com estoque negativo após dedução.");
            stockItem.StockQuantity = 0;
        }

        _context.Products.Update(stockItem);

        // Se "saveChanges" for true, salva as mudanças
        if (saveChanges)
            await _context.SaveChangesAsync();
    }
}