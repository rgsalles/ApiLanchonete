using ApiLanchonete.Authentication;
using ApiLanchonete.Features.Branches;
using ApiLanchonete.Features.Clients;
using ApiLanchonete.Features.Companies;
using ApiLanchonete.Features.Inventory;
using ApiLanchonete.Features.Orders;
using ApiLanchonete.Features.Payments;
using ApiLanchonete.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
