using EcommerceOrderManagement.Migrations.Models;
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
            .HasMaxLength(100); // Ajuste o tamanho conforme necessário
        
        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime");
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime");

        builder.HasOne(pp => pp.Order)
            .WithOne(o => o.PixPayment)
            .HasForeignKey<PixPayment>(pp => pp.OrderId)
            .OnDelete(DeleteBehavior.Restrict); // Ajuste conforme a lógica de negócios
    }
}