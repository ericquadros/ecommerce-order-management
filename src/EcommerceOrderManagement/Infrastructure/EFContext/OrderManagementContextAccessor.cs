using EcommerceOrderManagement.Infrastructure.EFContext;

namespace EcommerceOrderManagement.Infrastructure;

public sealed class OrderManagementContextAccessor : IEfDbContextAccessor<OrderManagementDbContext>
{
    private readonly AsyncLocal<OrderManagementDbContext> _contextAccessor = new();
    private bool _disposed;

    public OrderManagementDbContext Get()
    {
        return _contextAccessor.Value ?? throw new InvalidOperationException("Contexto must be registered!");
    }

    public void Register(OrderManagementDbContext context)
    {
        _disposed = false;
        _contextAccessor.Value = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Clear()
    {
        Dispose(true);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _contextAccessor.Value?.Dispose();

        _contextAccessor.Value = null!;
        _disposed = true;
    }
}

public interface IEfDbContextAccessor<T>
{
}