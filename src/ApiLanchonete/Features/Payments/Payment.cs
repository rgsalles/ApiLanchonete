using ApiLanchonete.Features.Orders;

namespace ApiLanchonete.Features.Payments;

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum PaymentMethod { Cash = 1, Pix = 2, CreditCard = 3, DebitCard = 4 }
public enum PaymentStatus { Pending = 1, Paid = 2, Refunded = 3, Cancelled = 4 }
