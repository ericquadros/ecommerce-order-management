namespace EcommerceOrderManagement.Infrastructure;

public abstract class Entity : IEntity
{
    public Entity()
    {
        Id = Guid.NewGuid();
        UpdatedAt = DateTime.Now;
    }

    public Guid Id { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public interface IEntity
{
    Guid Id { get; init; }
}