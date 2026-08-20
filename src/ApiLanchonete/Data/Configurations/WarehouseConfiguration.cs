using ApiLanchonete.Features.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiLanchonete.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder
            .HasOne(w => w.Branch)
            .WithMany(b => b.Warehouses)
            .HasForeignKey(w => w.BranchId);

        builder
            .HasOne(w => w.Product)
            .WithMany(p => p.Warehouses)
            .HasForeignKey(w => w.ProductId);

        builder
            .HasIndex(w => new { w.BranchId, w.ProductId })
            .IsUnique();
    }
}