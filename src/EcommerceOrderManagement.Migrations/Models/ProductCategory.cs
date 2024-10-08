namespace EcommerceOrderManagement.Migrations.Models;

public class ProductCategoryModel
{
    public Guid Id { get; set; }           // Unique identifier for the category
    public string Name { get; set; }       // Name of the category
    public string Description { get; set; } // Description of the category (optional)
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}