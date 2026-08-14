using ApiLanchonete.Features.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ApiLanchonete.Data.Configurations;
public class OrdermItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .Property(i => i.UnitPrice)
            .HasPrecision(10, 2);

        builder
            .Property(i => i.TotalPrice)
            .HasPrecision(10, 2);
        builder
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId);
        builder
            .HasOne(i => i.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId);
    }
}