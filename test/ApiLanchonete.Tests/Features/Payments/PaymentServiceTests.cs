using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using ApiLanchonete.Features.Orders;
using ApiLanchonete.Features.Payments;

namespace ApiLanchonete.Tests.Features.Payments;

public class PaymentServiceTests
{
    [Fact]
    public async Task CreatePayment_CreatesPendingPaymentWhenAmountMatchesOrder()
    {
        await using var context = TestDbContextFactory.Create();
        var order = await AddOrder(context, total: 42.50m);
        var service = new PaymentService(context);

        var payment = await service.CreatePayment(new CreatePaymentDto
        {
            OrderId = order.Id,
            Method = PaymentMethod.Pix,
            Amount = 42.50m
        });

        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(PaymentMethod.Pix, payment.Method);
    }

    [Fact]
    public async Task CreatePayment_RejectsAmountDifferentFromOrderTotal()
    {
        await using var context = TestDbContextFactory.Create();
        var order = await AddOrder(context, total: 20m);
        var service = new PaymentService(context);

        await Assert.ThrowsAsync<BadRequestException>(() => service.CreatePayment(new CreatePaymentDto
        {
            OrderId = order.Id,
            Method = PaymentMethod.Cash,
            Amount = 19m
        }));
    }

    [Fact]
    public async Task CreatePayment_RejectsSecondPaymentForSameOrder()
    {
        await using var context = TestDbContextFactory.Create();
        var order = await AddOrder(context, total: 20m);
        var service = new PaymentService(context);
        var dto = new CreatePaymentDto { OrderId = order.Id, Method = PaymentMethod.DebitCard, Amount = 20m };

        await service.CreatePayment(dto);

        await Assert.ThrowsAsync<ConflictException>(() => service.CreatePayment(dto));
    }

    [Fact]
    public async Task UpdatePaymentStatus_ToPaidSetsPaidAt()
    {
        await using var context = TestDbContextFactory.Create();
        var order = await AddOrder(context, total: 20m);
        var service = new PaymentService(context);
        var payment = await service.CreatePayment(new CreatePaymentDto
        {
            OrderId = order.Id, Method = PaymentMethod.CreditCard, Amount = 20m
        });

        await service.UpdatePaymentStatus(payment.Id, new UpdatePaymentStatusDto { Status = PaymentStatus.Paid });
        var updated = await service.GetPaymentByOrderId(order.Id);

        Assert.Equal(PaymentStatus.Paid, updated.Status);
        Assert.NotNull(updated.PaidAt);
    }

    private static async Task<Order> AddOrder(AppDbContext context, decimal total)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(), BranchId = Guid.NewGuid(), ClientId = Guid.NewGuid(),
            Status = OrderStatus.Pending, TotalAmount = total, CreatedAt = DateTime.UtcNow
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }
}
