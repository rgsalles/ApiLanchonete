using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Features.Inventories;

namespace ApiLanchonete.Tests.Features.Inventories;

public class InventoriesServiceTests
{
    [Fact]
    public async Task CreateInventory_TrimsValuesAndSetsAuditData()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new InventoryService(context);

        var inventory = await service.CreateInventory(new CreateInventoryDto
        {
            Name = "  Estoque Central  ", Description = "  Estoque principal da lanchonete  "
        });

        Assert.Equal("Estoque Central", inventory.Name);
        Assert.Equal("Estoque principal da lanchonete", inventory.Description);
        Assert.Equal("System", inventory.CreatedBy);
    }

    [Fact]
    public async Task CreateInventory_RejectsDuplicateName()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new InventoryService(context);
        var dto = new CreateInventoryDto { Name = "Estoque", Description = "Descrição" };
        await service.CreateInventory(dto);

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateInventory(new CreateInventoryDto
        {
            Name = dto.Name, Description = "Outra descrição"
        }));
    }
}