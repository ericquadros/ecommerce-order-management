using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.Domain.Infrastructure.EFContext;

public interface IEfDbContextAccessor<T> : IDisposable where T : DbContext
{
    void Register(T context);
    T Get();
    void Clear();
}