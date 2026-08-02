namespace ApiLanchonete.Features.Clients
{
    public interface IClientService
    {
        Task<List<ClientDto>> GetClients();
        Task<ClientDto?> GetClientById(Guid id);
        Task<bool> UpdateClient(Guid id, UpdateClientDto dto);
        Task<bool> DeleteClient(Guid id);

    }
}
