using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Features.Warehouses;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid ProductId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinimumQuantity { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateWarehouseDto
{
    [Required]
    public Guid BranchId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, int.MaxValue)]
    public int MinimumQuantity { get; set; }
}

public class UpdateWarehouseDto
{
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, int.MaxValue)]
    public int MinimumQuantity { get; set; }

    public bool Active { get; set; }
}