using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Features.Warehouses;

[Authorize(Roles = "Admin,Staff")]
[ApiController]
[Route("api/[controller]")]
public class WarehouseController(IWarehouseService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<WarehouseDto>>> GetWarehouses([FromQuery] Guid? branchId)
        => Ok(await service.GetWarehouses(branchId));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WarehouseDto>> GetWarehouse(Guid id)
        => Ok(await service.GetWarehouseById(id));

    [HttpPost]
    public async Task<ActionResult<WarehouseDto>> CreateWarehouse(CreateWarehouseDto dto)
    {
        var warehouse = await service.CreateWarehouse(dto);
        return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouse);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWarehouse(Guid id, UpdateWarehouseDto dto)
    {
        await service.UpdateWarehouse(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWarehouse(Guid id)
    {
        await service.DeleteWarehouse(id);
        return NoContent();
    }
}
