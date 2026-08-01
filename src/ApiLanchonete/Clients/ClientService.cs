using ApiLanchonete.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Clients;

public class ClientService(
    AppDbContext context,
    IPasswordHasher<Client> passwordHasher) : IClientService
{
    public async Task<List<ClientDto>> GetClients()
    {
        return await context.Clients
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
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
        var client = await context.Clients.FindAsync(id);

        if (client == null)
            return null;

        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
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

    public async Task<ClientDto> CreateClient(CreateClientDto dto)
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            CEP = dto.CEP,
            Country = dto.Country,
            Phone = dto.Phone,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        client.PasswordHash = passwordHasher.HashPassword(client, dto.Password);

        context.Clients.Add(client);

        await context.SaveChangesAsync();

        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
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
        client.Email = dto.Email;
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