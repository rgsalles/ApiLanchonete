using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Features.Companies;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class CompanyController(ICompanyService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> GetCompanies() => Ok(await service.GetCompanies());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(Guid id) => Ok(await service.GetCompanyById(id));

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto dto)
    {
        var company = await service.CreateCompany(dto);
        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid id, UpdateCompanyDto dto)
    {
        await service.UpdateCompany(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await service.DeleteCompany(id);
        return NoContent();
    }
}
