using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrderManagement.Migrations.EFMappings;

public class OrderMapping : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(o => o.OrderDate)
            .HasColumnType("datetime");
        
        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime");
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime");
        
        // Configurando the relationship with Customer
        builder.HasOne(o => o.Customer)
            .WithMany() // An Customer can has many Orders
            .HasForeignKey(o => o.CustomerId) // Foreign key
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship with OrderItem
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi=>oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}