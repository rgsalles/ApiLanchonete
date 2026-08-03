using ApiLanchonete.Features.Clients;

namespace ApiLanchonete.Features.Orders;

public class Order
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public ApiLanchonete.Features.Branches.Branch Branch { get; set; } = null!;
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public bool StockReserved { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
    public ApiLanchonete.Features.Payments.Payment? Payment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public enum OrderStatus
{
    Pending = 1,
    Preparing = 2,
    Ready = 3,
    Delivered = 4,
    Cancelled = 5
}
