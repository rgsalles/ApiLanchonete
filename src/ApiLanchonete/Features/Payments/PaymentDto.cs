using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Features.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class CreatePaymentDto
{
    [Required] public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
}

public class UpdatePaymentStatusDto { public PaymentStatus Status { get; set; } }
