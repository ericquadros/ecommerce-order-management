using EcommerceOrderManagement.Infrastructure.EFContext;

namespace EcommerceOrderManagement.Domain.Infrastructure;

public sealed class DigitacaoDbContextAccessor: IEfDbContextAccessor<OrderManagementDbContext>
{
    private OrderManagementDbContext _contexto = null!;
    private bool _disposed = false;

    public OrderManagementDbContext Get()
    {
        return _contexto ?? throw new InvalidOperationException("Contexto must be registered!");
    }

    public void Register(OrderManagementDbContext context)
    {
        _disposed = false;
        _contexto = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Clear()
    {
        Dispose(true);
    }

    public void Dispose()
    {
        Dispose(true);
        // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _contexto?.Dispose();

        _contexto = null!;
        _disposed = true;
    }
}

public interface IEfDbContextAccessor<T>
{
}