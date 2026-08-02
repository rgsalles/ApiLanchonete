using ApiLanchonete.Authentication;
using ApiLanchonete.Features.Clients;
using ApiLanchonete.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Client>()
            .HasOne(c => c.User)
            .WithOne(u => u.Client)
            .HasForeignKey<Client>(c => c.UserId);

        base.OnModelCreating(modelBuilder);
    }
}