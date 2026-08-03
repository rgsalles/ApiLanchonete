namespace ApiLanchonete.Features.Inventory;

public interface IInventoryService
{
    Task<List<InventoryDto>> GetInventories(Guid? branchId = null);
    Task<InventoryDto> GetInventoryById(Guid id);
    Task<InventoryDto> CreateInventory(CreateInventoryDto dto);
    Task UpdateInventory(Guid id, UpdateInventoryDto dto);
    Task DeleteInventory(Guid id);
}
