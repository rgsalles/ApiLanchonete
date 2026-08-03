namespace ApiLanchonete.Features.Payments;

public interface IPaymentService
{
    Task<PaymentDto> GetPaymentByOrderId(Guid orderId);
    Task<PaymentDto> CreatePayment(CreatePaymentDto dto);
    Task UpdatePaymentStatus(Guid id, UpdatePaymentStatusDto dto);
}
