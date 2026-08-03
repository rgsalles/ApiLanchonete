using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using ApiLanchonete.Features.Orders;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Payments;

public class PaymentService(AppDbContext context) : IPaymentService
{
    public async Task<PaymentDto> GetPaymentByOrderId(Guid orderId)
    {
        var payment = await context.Payments.AsNoTracking()
            .FirstOrDefaultAsync(payment => payment.OrderId == orderId)
            ?? throw new NotFoundException($"Payment for order {orderId} not found.");
        return ToDto(payment);
    }

    public async Task<PaymentDto> CreatePayment(CreatePaymentDto dto)
    {
        var order = await context.Orders.FindAsync(dto.OrderId)
            ?? throw new NotFoundException($"Order with ID {dto.OrderId} not found.");

        if (order.Status == OrderStatus.Cancelled)
            throw new BadRequestException("A cancelled order cannot be paid.");

        if (await context.Payments.AnyAsync(payment => payment.OrderId == dto.OrderId))
            throw new ConflictException("This order already has a payment.");

        if (dto.Amount != order.TotalAmount)
            throw new BadRequestException("The payment amount must match the order total.");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            Method = dto.Method,
            Status = PaymentStatus.Pending,
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();
        return ToDto(payment);
    }

    public async Task UpdatePaymentStatus(Guid id, UpdatePaymentStatusDto dto)
    {
        var payment = await context.Payments.Include(payment => payment.Order)
            .FirstOrDefaultAsync(payment => payment.Id == id)
            ?? throw new NotFoundException($"Payment with ID {id} not found.");

        if (payment.Status is PaymentStatus.Refunded or PaymentStatus.Cancelled)
            throw new BadRequestException("A refunded or cancelled payment cannot be changed.");

        if (dto.Status == PaymentStatus.Paid)
            payment.PaidAt = DateTime.UtcNow;

        payment.Status = dto.Status;
        payment.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    private static PaymentDto ToDto(Payment payment) => new()
    {
        Id = payment.Id,
        OrderId = payment.OrderId,
        Method = payment.Method,
        Status = payment.Status,
        Amount = payment.Amount,
        CreatedAt = payment.CreatedAt,
        PaidAt = payment.PaidAt
    };
}
