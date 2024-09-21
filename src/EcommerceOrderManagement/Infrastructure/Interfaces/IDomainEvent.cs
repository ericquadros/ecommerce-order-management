namespace EcommerceOrderManagement.Infrastructure.Interfaces;

public interface IDomainEvent<T>
{
    string EventName { get; }
    T Object { get; }
    DateTime OccurredOn { get; }
}