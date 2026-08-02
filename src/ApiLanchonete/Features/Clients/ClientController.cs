using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLanchonete.Features.Clients;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientController(IClientService clientService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetClients()
    {
        var clients = await clientService.GetClients();

        return Ok(clients);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetClientById(Guid id)
    {
        var client = await clientService.GetClientById(id);

        return Ok(client);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateClient(
        Guid id,
        UpdateClientDto dto)
    {
        await clientService.UpdateClient(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        await clientService.DeleteClient(id);

        return NoContent();
    }
}