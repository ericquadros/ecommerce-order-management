using EcommerceOrderManagement.Infrastructure.Interfaces;

namespace EcommerceOrderManagement.Infrastructure.Interfaces;

public interface IMessageBroker
{
    Task ProduceMessageAsync<T>(IDomainEvent<T> domainEvent) 
        where T : Entity, IEntity;
}
