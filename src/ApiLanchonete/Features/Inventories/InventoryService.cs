using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Inventory;

public class InventoryService(AppDbContext context) : IInventoryService
{
    public async Task<List<InventoryDto>> GetInventories(Guid? branchId = null)
    {
        var query = context.Inventories
            .Include(i => i.Branch)
            .Include(i => i.Product)
            .AsNoTracking();

        if (branchId.HasValue)
            query = query.Where(inventory => inventory.BranchId == branchId.Value);

        var inventories = await query.ToListAsync();

        return inventories.Select(ToDto).ToList();
    }

    public async Task<InventoryDto> GetInventoryById(Guid id)
    {
        var inventory = await context.Inventories
            .Include(i => i.Branch)
            .Include(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (inventory is null)
            throw new NotFoundException($"Inventory with ID {id} not found.");

        return ToDto(inventory);
    }

    public async Task<InventoryDto> CreateInventory(CreateInventoryDto dto)
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

        var exists = await context.Inventories.AnyAsync(i =>
            i.BranchId == dto.BranchId &&
            i.ProductId == dto.ProductId);

        if (exists)
            throw new ConflictException("This product is already registered for this branch.");

        var inventory = new Inventory
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

        context.Inventories.Add(inventory);

        await context.SaveChangesAsync();

        inventory = await context.Inventories
            .Include(i => i.Branch)
            .Include(i => i.Product)
            .FirstAsync(i => i.Id == inventory.Id);

        return ToDto(inventory);
    }

    public async Task UpdateInventory(Guid id, UpdateInventoryDto dto)
    {
        var inventory = await context.Inventories.FindAsync(id);

        if (inventory is null)
            throw new NotFoundException($"Inventory with ID {id} not found.");

        inventory.Quantity = dto.Quantity;
        inventory.MinimumQuantity = dto.MinimumQuantity;
        inventory.Active = dto.Active;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteInventory(Guid id)
    {
        var inventory = await context.Inventories.FindAsync(id);

        if (inventory is null)
            throw new NotFoundException($"Inventory with ID {id} not found.");

        context.Inventories.Remove(inventory);

        await context.SaveChangesAsync();
    }

    private static InventoryDto ToDto(Inventory inventory)
    {
        return new InventoryDto
        {
            Id = inventory.Id,
            BranchId = inventory.BranchId,
            ProductId = inventory.ProductId,
            BranchName = inventory.Branch.Name,
            ProductName = inventory.Product.Name,
            Quantity = inventory.Quantity,
            MinimumQuantity = inventory.MinimumQuantity,
            Active = inventory.Active,
            CreatedAt = inventory.CreatedAt,
            CreatedBy = inventory.CreatedBy,
            UpdatedAt = inventory.UpdatedAt,
            UpdatedBy = inventory.UpdatedBy
        };
    }
}
