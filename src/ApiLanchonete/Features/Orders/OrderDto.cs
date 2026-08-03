using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Features.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid ClientId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateOrderDto
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid BranchId { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = [];
}

public class UpdateOrderDto
{
    [Required]
    public OrderStatus Status { get; set; }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
