using CashFlowSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlowSystem.Infrastructure.Data.Configurations;

public class CashRegisterConfiguration : IEntityTypeConfiguration<CashRegister>
{
    public void Configure(EntityTypeBuilder<CashRegister> builder)
    {
        builder.ToTable("cash_registers");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.OpeningDate)
            .IsRequired();

        builder.Property(cr => cr.OpeningBalance)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(cr => cr.ClosingBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(cr => cr.ExpectedBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(cr => cr.Difference)
            .HasColumnType("decimal(18,2)");

        builder.Property(cr => cr.Status)
            .IsRequired()
            .HasConversion<int>();

        // Relationships
        builder.HasOne(cr => cr.User)
            .WithMany(u => u.CashRegisters)
            .HasForeignKey(cr => cr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(cr => cr.OpeningDate);
        builder.HasIndex(cr => cr.Status);
        builder.HasIndex(cr => cr.UserId);
    }
}
