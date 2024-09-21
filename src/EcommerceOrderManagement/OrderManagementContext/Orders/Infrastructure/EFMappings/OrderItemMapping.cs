using EcommerceOrderManagement.Migrations.Models;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrderManagement.Migrations.EFMappings;

public class OrderItemMapping : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime");
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime");
        
        // // Configurando a chave estrangeira para Order
        // builder.HasOne<Order>()
        //     .WithMany() // Um pedido tem muitos itens
        //     .HasForeignKey(oi => oi.OrderId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(o => o.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}