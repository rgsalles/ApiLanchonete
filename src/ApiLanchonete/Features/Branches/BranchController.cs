using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Features.Branches;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class BranchController(IBranchService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BranchDto>>> GetBranches([FromQuery] Guid? companyId)
        => Ok(await service.GetBranches(companyId));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BranchDto>> GetBranch(Guid id) => Ok(await service.GetBranchById(id));

    [HttpPost]
    public async Task<ActionResult<BranchDto>> CreateBranch(CreateBranchDto dto)
    {
        var branch = await service.CreateBranch(dto);
        return CreatedAtAction(nameof(GetBranch), new { id = branch.Id }, branch);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBranch(Guid id, UpdateBranchDto dto)
    {
        await service.UpdateBranch(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBranch(Guid id)
    {
        await service.DeleteBranch(id);
        return NoContent();
    }
}
