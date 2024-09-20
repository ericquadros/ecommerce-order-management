using EcommerceOrderManagement.Migrations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrderManagement.Migrations.EFMappings;

public class OrderItemMapping : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
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
        builder.HasOne<Product>(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship with Order
        builder.HasOne<Order>()
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
