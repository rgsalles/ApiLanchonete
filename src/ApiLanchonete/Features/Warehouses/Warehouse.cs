using ApiLanchonete.Features.Branches;
using ApiLanchonete.Features.Products;

namespace ApiLanchonete.Features.Warehouses;

public class Warehouse
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public int MinimumQuantity { get; set; }
    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
