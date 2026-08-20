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
            .Include(w => w.Product)
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
            .Include(w => w.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse is null)
            throw new NotFoundException($"Warehouse with ID {id} not found.");

        return ToDto(warehouse);
    }

    public async Task<WarehouseDto> CreateWarehouse(CreateWarehouseDto dto)
    {
        var branchAndProduct = await (
            from branch in context.Branches
            join product in context.Products on branch.CompanyId equals product.CompanyId
            where branch.Id == dto.BranchId && product.Id == dto.ProductId
            select new { Branch = branch, Product = product })
            .FirstOrDefaultAsync();

        if (branchAndProduct is null)
        {
            var branchExists = await context.Branches.AnyAsync(branch => branch.Id == dto.BranchId);
            if (!branchExists)
                throw new NotFoundException($"Branch with ID {dto.BranchId} not found.");

            var productExists = await context.Products.AnyAsync(product => product.Id == dto.ProductId);
            if (!productExists)
                throw new NotFoundException($"Product with ID {dto.ProductId} not found.");

            throw new BadRequestException("The product must belong to the same company as the branch.");
        }

        var exists = await context.Warehouses.AnyAsync(w =>
            w.BranchId == dto.BranchId &&
            w.ProductId == dto.ProductId);

        if (exists)
            throw new ConflictException("This product is already registered for this branch in the warehouse.");

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            BranchId = dto.BranchId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            MinimumQuantity = dto.MinimumQuantity,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Warehouses.Add(warehouse);
        await context.SaveChangesAsync();

        warehouse = await context.Warehouses
            .Include(w => w.Branch)
            .Include(w => w.Product)
            .FirstAsync(w => w.Id == warehouse.Id);

        return ToDto(warehouse);
    }

    public async Task UpdateWarehouse(Guid id, UpdateWarehouseDto dto)
    {
        var warehouse = await context.Warehouses.FindAsync(id);

        if (warehouse is null)
            throw new NotFoundException($"Warehouse with ID {id} not found.");

        warehouse.Quantity = dto.Quantity;
        warehouse.MinimumQuantity = dto.MinimumQuantity;
        warehouse.Active = dto.Active;
        warehouse.UpdatedAt = DateTime.UtcNow;
        warehouse.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteWarehouse(Guid id)
    {
        var warehouse = await context.Warehouses.FindAsync(id);

        if (warehouse is null)
            throw new NotFoundException($"Warehouse with ID {id} not found.");

        context.Warehouses.Remove(warehouse);
        await context.SaveChangesAsync();
    }

    private static WarehouseDto ToDto(Warehouse warehouse) => new()
    {
        Id = warehouse.Id,
        BranchId = warehouse.BranchId,
        ProductId = warehouse.ProductId,
        BranchName = warehouse.Branch.Name,
        ProductName = warehouse.Product.Name,
        Quantity = warehouse.Quantity,
        MinimumQuantity = warehouse.MinimumQuantity,
        Active = warehouse.Active,
        CreatedAt = warehouse.CreatedAt,
        CreatedBy = warehouse.CreatedBy,
        UpdatedAt = warehouse.UpdatedAt,
        UpdatedBy = warehouse.UpdatedBy
    };
}
