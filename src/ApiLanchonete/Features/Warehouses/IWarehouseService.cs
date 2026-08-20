namespace ApiLanchonete.Features.Warehouses;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetWarehouses(Guid? branchId = null);
    Task<WarehouseDto> GetWarehouseById(Guid id);
    Task<WarehouseDto> CreateWarehouse(CreateWarehouseDto dto);
    Task UpdateWarehouse(Guid id, UpdateWarehouseDto dto);
    Task DeleteWarehouse(Guid id);
}
