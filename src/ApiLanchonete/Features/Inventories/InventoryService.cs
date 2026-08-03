using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Inventories
{
    public class InventoryService(AppDbContext context) : IInventoryService
    {
        public async Task<List<InventoryDto>> GetInventories()
        {
            var inventories = await context.Inventories
                .AsNoTracking()
                .ToListAsync();
            return inventories.Select(ToDto).ToList();
        }

        private static InventoryDto ToDto(Inventory inventory)
        {
            return new InventoryDto
            {
                Id = inventory.Id,
                BranchId = inventory.BranchId,
                ProductId = inventory.ProductId,
                BranchName = inventory.BranchName,
                ProductName = inventory.ProductName,
                Quantity = inventory.Quantity,
                MinimumQuantity = inventory.MinimumQuantity,
                Active = inventory.Active,
                CreatedAt = inventory.CreatedAt,
                CreatedBy = inventory.CreatedBy,
                UpdatedAt = inventory.UpdatedAt,
                UpdatedBy = inventory.UpdatedBy,
            };
        }
    }
}
