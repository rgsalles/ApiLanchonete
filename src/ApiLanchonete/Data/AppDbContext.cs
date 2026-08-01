using ApiLanchonete.Products;
using ApiLanchonete.Clients;
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
}