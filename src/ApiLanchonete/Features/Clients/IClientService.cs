namespace ApiLanchonete.Features.Clients;

public interface IClientService
{
    Task<List<ClientDto>> GetClients();

    Task<ClientDto> GetClientById(Guid id);

    Task UpdateClient(Guid id, UpdateClientDto dto);

    Task DeleteClient(Guid id);
}