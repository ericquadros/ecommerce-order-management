using EcommerceOrderManagement.Domain.Infrastructure;

namespace EcommerceOrderManagement.Migrations.Models;

public class ProductCategory : Entity
{
    public string Name { get; set; }       
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}