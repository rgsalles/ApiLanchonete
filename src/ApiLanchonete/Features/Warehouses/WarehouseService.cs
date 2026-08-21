using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Warehouses;

public class WarehouseService(AppDbContext context) : IWarehouseService
{
    public async Task<List<WarehouseDto>> GetWarehouses(Guid? branchId = null)
    {
        var query = context.Warehouses
            .Include(w => w.Branch)
            .Include(w => w.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking();

        if (branchId.HasValue)
            query = query.Where(w => w.BranchId == branchId.Value);

        var warehouses = await query.ToListAsync();

        return warehouses.Select(ToDto).ToList();
    }

    public async Task<WarehouseDto> GetWarehouseById(Guid id)
    {
        var warehouse = await context.Warehouses
            .Include(w => w.Branch)
            .Include(w => w.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse is null)
            throw new NotFoundException(
                $"Warehouse with ID {id} not found.");

        return ToDto(warehouse);
    }

    public async Task<WarehouseDto> CreateWarehouse(CreateWarehouseDto dto)
    {
        var branchExists = await context.Branches
            .AnyAsync(b => b.Id == dto.BranchId);

        if (!branchExists)
            throw new NotFoundException(
                $"Branch with ID {dto.BranchId} not found.");

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            BranchId = dto.BranchId,
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Active = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Warehouses.Add(warehouse);

        await context.SaveChangesAsync();

        warehouse = await context.Warehouses
            .Include(w => w.Branch)
            .Include(w => w.Items)
                .ThenInclude(i => i.Product)
            .FirstAsync(w => w.Id == warehouse.Id);

        return ToDto(warehouse);
    }

    public async Task UpdateWarehouse(Guid id, UpdateWarehouseDto dto)
    {
        var warehouse = await context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse is null)
            throw new NotFoundException(
                $"Warehouse with ID {id} not found.");

        warehouse.Code = dto.Code.Trim();
        warehouse.Name = dto.Name.Trim();
        warehouse.Active = dto.Active;
        warehouse.UpdatedAt = DateTime.UtcNow;
        warehouse.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteWarehouse(Guid id)
    {
        var warehouse = await context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse is null)
            throw new NotFoundException(
                $"Warehouse with ID {id} not found.");

        context.Warehouses.Remove(warehouse);

        await context.SaveChangesAsync();
    }

    private static WarehouseDto ToDto(Warehouse warehouse) => new()
    {
        Id = warehouse.Id,
        BranchId = warehouse.BranchId,
        BranchName = warehouse.Branch.Name,
        Code = warehouse.Code,
        Name = warehouse.Name,
        Active = warehouse.Active,
        CreatedAt = warehouse.CreatedAt,
        CreatedBy = warehouse.CreatedBy,
        UpdatedAt = warehouse.UpdatedAt,
        UpdatedBy = warehouse.UpdatedBy,

        Items = warehouse.Items
            .Select(item => new WarehouseItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                MinimumQuantity = item.MinimumQuantity,
                Active = item.Active
            })
            .ToList()
    };
}