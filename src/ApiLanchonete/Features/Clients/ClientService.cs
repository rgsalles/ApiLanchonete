using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Clients;

public class ClientService(AppDbContext context) : IClientService
{
    public async Task<List<ClientDto>> GetClients()
    {
        return await context.Clients
            .AsNoTracking()
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                City = c.City,
                State = c.State,
                CEP = c.CEP,
                Country = c.Country,
                Phone = c.Phone,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy
            })
            .ToListAsync();
    }

    public async Task<ClientDto?> GetClientById(Guid id)
    {
        var client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return null;

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
    public async Task<bool> UpdateClient(Guid id, UpdateClientDto dto)
    {
        var client = await context.Clients.FindAsync(id);

        if (client == null)
            return false;

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

        return true;
    }

    public async Task<bool> DeleteClient(Guid id)
    {
        var client = await context.Clients.FindAsync(id);

        if (client == null)
            return false;

        context.Clients.Remove(client);

        await context.SaveChangesAsync();

        return true;
    }
}