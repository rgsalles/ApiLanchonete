using ApiLanchonete.Features.Branches;

namespace ApiLanchonete.Features.Warehouses;

public class Warehouse
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public bool Active { get; set; } = true;

    public ICollection<WarehouseItem> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}