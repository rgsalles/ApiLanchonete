using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Features.Warehouses;

public class WarehouseDto
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; }

    public List<WarehouseItemDto> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class WarehouseItemDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public int MinimumQuantity { get; set; }

    public bool Active { get; set; }
}

public class CreateWarehouseDto
{
    [Required]
    public Guid BranchId { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateWarehouseDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; }
}