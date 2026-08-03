using ApiLanchonete.Features.Orders;
using ApiLanchonete.Features.Companies;
using InventoryItem = ApiLanchonete.Features.Inventory.Inventory;

namespace ApiLanchonete.Features.Branches;

public class Branch
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    // Relationships
    public Company Company { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<InventoryItem> Inventory { get; set; } = [];

    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
