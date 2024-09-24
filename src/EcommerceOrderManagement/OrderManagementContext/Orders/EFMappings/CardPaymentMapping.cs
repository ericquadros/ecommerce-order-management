using EcommerceOrderManagement.Migrations.Models;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceOrderManagement.Migrations.EFMappings;


public class CardPaymentMapping : IEntityTypeConfiguration<CardPayment>
{
    public void Configure(EntityTypeBuilder<CardPayment> builder)
    {
        builder.ToTable("CardPayments");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.CardNumber)
            .IsRequired()
            .HasColumnType("varchar(16)")
            .HasMaxLength(16);

        builder.Property(cp => cp.CardHolder)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(cp => cp.ExpirationDate)
            .IsRequired()
            .HasColumnType("varchar(5)")
            .HasMaxLength(5); // Format MM/AA

        builder.Property(cp => cp.Cvv)
            .IsRequired()
            .HasColumnType("varchar(4)")
            .HasMaxLength(4);
        
        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime");
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime");

        builder.HasOne<Order>()
            .WithOne(o => o.CardPayment)
            .HasForeignKey<CardPayment>(cp => cp.OrderId)
            .OnDelete(DeleteBehavior.Restrict); // Ajuste conforme a lógica de negócios
    }
}