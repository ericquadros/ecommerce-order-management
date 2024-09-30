using EcommerceOrderManagement.Migrations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrderManagement.Migrations.EFMappings;

public class OrderItemMapping : IEntityTypeConfiguration<OrderItemModel>
{
    public void Configure(EntityTypeBuilder<OrderItemModel> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.Quantity)
            .HasColumnType("int");
        
        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime");
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime");

        // Configure the relationship with Product
        builder.HasOne<ProductModel>(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship with Order
        builder.HasOne<OrderModel>()
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
