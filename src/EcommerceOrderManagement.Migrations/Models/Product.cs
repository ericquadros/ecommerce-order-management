namespace EcommerceOrderManagement.Migrations.Models;

public class Product
{
    public Guid Id { get; set; }      
    public string Name { get; set; }  
    public string Description { get; set; }
    public decimal Price { get; set; }     
    public Guid CategoryId { get; set; } 
    public ProductCategory Category { get; set; } 

    public string ImageUrl { get; set; }
}