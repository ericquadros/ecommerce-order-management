
using EcommerceOrderManagement.Infrastructure;

namespace EcommerceOrderManagement.Migrations.Models;

public class Product : Entity
{
    public Product(string name, string description, decimal price, ProductCategory category, string imageUrl)
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        ImageUrl = imageUrl;
    }
    
    private Product() // EF
    { }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    internal Guid CategoryId { get; } 
    public ProductCategory Category { get;  private set; } 
    
    public Owner Owner { get; set; } 
    public int StockQuantity { get; set; }
    public string ImageUrl { get; private set; }
}

public record Owner(string OwnerName, string OwnerEmail);
