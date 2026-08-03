using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Features.Inventory;

[Authorize(Roles = "Admin,Staff")]
[ApiController]
[Route("api/[controller]")]
public class InventoryController(IInventoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<InventoryDto>>> GetInventories([FromQuery] Guid? branchId)
        => Ok(await service.GetInventories(branchId));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InventoryDto>> GetInventory(Guid id)
        => Ok(await service.GetInventoryById(id));

    [HttpPost]
    public async Task<ActionResult<InventoryDto>> CreateInventory(CreateInventoryDto dto)
    {
        var inventory = await service.CreateInventory(dto);
        return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateInventory(Guid id, UpdateInventoryDto dto)
    {
        await service.UpdateInventory(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteInventory(Guid id)
    {
        await service.DeleteInventory(id);
        return NoContent();
    }
}
