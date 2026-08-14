using ApiLanchonete.Features.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiLanchonete.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder
            .HasOne(i => i.Branch)
            .WithMany(b => b.Inventory)
            .HasForeignKey(i => i.BranchId);

        builder
            .HasOne(i => i.Product)
            .WithMany(p => p.Inventory)
            .HasForeignKey(i => i.ProductId);

        builder
            .HasIndex(i => new { i.BranchId, i.ProductId })
            .IsUnique();
    }
}