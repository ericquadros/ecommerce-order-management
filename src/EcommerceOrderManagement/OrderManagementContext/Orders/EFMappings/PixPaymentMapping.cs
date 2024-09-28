using EcommerceOrderManagement.Migrations.Models;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrderManagement.Migrations.EFMappings;

public class PixPaymentMapping : IEntityTypeConfiguration<PixPayment>
{
    public void Configure(EntityTypeBuilder<PixPayment> builder)
    {
        builder.ToTable("PixPayments");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.TransactionId)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);
        
        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime");
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime");

        builder.HasOne<Order>()
            .WithOne(o => o.PixPayment)
            .HasForeignKey<PixPayment>(pp => pp.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}