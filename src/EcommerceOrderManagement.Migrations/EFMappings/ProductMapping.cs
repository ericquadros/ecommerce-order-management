using EcommerceOrderManagement.Migrations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrderManagement.Migrations.EFMappings;

public class ProductMapping: IEntityTypeConfiguration<ProductModel>
{
    public void Configure(EntityTypeBuilder<ProductModel> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");

        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .HasColumnType("varchar(500)");

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(255)
            .HasColumnType("varchar(255)");
        
        builder.Property(p => p.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.OwnerName)
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");

        builder.Property(p => p.OwnerEmail)
            .HasMaxLength(255)
            .HasColumnType("varchar(255)");
        
        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime");
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime");

        // Configure the relationship to ProductCategory
        builder.HasOne(p => p.Category)
            .WithMany() // No navigation property on ProductCategory
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}