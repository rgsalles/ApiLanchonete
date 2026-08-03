using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Features.Payments;

[Authorize(Roles = "Admin,Staff")]
[ApiController]
[Route("api/[controller]")]
public class PaymentController(IPaymentService service) : ControllerBase
{
    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(Guid orderId) => Ok(await service.GetPaymentByOrderId(orderId));

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment(CreatePaymentDto dto)
    {
        var payment = await service.CreatePayment(dto);
        return CreatedAtAction(nameof(GetPayment), new { orderId = payment.OrderId }, payment);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdatePaymentStatusDto dto)
    {
        await service.UpdatePaymentStatus(id, dto);
        return NoContent();
    }
}
