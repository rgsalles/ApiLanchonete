using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Features.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public bool Active { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateProductDto
{
    [Required]
    public Guid CompanyId { get; set; }

    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }
}

public class UpdateProductDto
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }
}
