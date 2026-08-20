using ApiLanchonete.Features.Companies;
using WarehouseItem = ApiLanchonete.Features.Warehouses.Warehouse;
using ApiLanchonete.Features.Orders;

namespace ApiLanchonete.Features.Products;

public class Product
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public bool Active { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }

    // Orders Relationship
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<WarehouseItem> Warehouses { get; set; } = [];

    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
