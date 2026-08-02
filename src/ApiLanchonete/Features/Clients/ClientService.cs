using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Clients;

public class ClientService(AppDbContext context) : IClientService
{
    public async Task<List<ClientDto>> GetClients()
    {
        var clients = await context.Clients
            .AsNoTracking()
            .ToListAsync();

        return clients.Select(ToDto).ToList();
    }

    public async Task<ClientDto> GetClientById(Guid id)
    {
        var client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client is null)
            throw new NotFoundException($"Client with ID {id} not found.");

        return ToDto(client);
    }

    public async Task UpdateClient(Guid id, UpdateClientDto dto)
    {
        var client = await context.Clients.FindAsync(id);

        if (client is null)
            throw new NotFoundException($"Client with ID {id} not found.");

        client.Name = dto.Name;
        client.Address = dto.Address;
        client.City = dto.City;
        client.State = dto.State;
        client.CEP = dto.CEP;
        client.Country = dto.Country;
        client.Phone = dto.Phone;

        client.UpdatedAt = DateTime.UtcNow;
        client.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteClient(Guid id)
    {
        var client = await context.Clients.FindAsync(id);

        if (client is null)
            throw new NotFoundException($"Client with ID {id} not found.");

        context.Clients.Remove(client);

        await context.SaveChangesAsync();
    }

    private static ClientDto ToDto(Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Address = client.Address,
            City = client.City,
            State = client.State,
            CEP = client.CEP,
            Country = client.Country,
            Phone = client.Phone,
            CreatedAt = client.CreatedAt,
            CreatedBy = client.CreatedBy,
            UpdatedAt = client.UpdatedAt,
            UpdatedBy = client.UpdatedBy
        };
    }
}