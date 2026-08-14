using ApiLanchonete.Features.Branches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ApiLanchonete.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder
            .HasIndex(b => new { b.CompanyId, b.Name })
            .IsUnique();
    }
}