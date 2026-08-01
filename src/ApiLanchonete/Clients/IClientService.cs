namespace ApiLanchonete.Clients
{
    public interface IClientService
    {
        Task<List<ClientDto>> GetClients();
        Task<ClientDto?> GetClientById(Guid id);
        Task<ClientDto> CreateClient(CreateClientDto dto);
        Task<bool> UpdateClient(Guid id, UpdateClientDto dto);
        Task<bool> DeleteClient(Guid id);

    }
}
