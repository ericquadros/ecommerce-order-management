namespace EcommerceOrderManagement.Migrations.Models;

public class ProductModel
{
    public Guid Id { get; set; }      
    public string Name { get; set; }  
    public string Description { get; set; }
    public decimal Price { get; set; }     
    public Guid CategoryId { get; set; } 
    public ProductCategoryModel Category { get; set; } 

    public string ImageUrl { get; set; }
    
    public int StockQuantity { get; set; } 
    
    public string OwnerName { get; set; }
    public string OwnerEmail { get; set; } 
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}