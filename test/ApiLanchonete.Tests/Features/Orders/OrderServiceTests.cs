using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using ApiLanchonete.Features.Branches;
using ApiLanchonete.Features.Clients;
using ApiLanchonete.Features.Companies;
using ApiLanchonete.Features.Inventory;
using ApiLanchonete.Features.Orders;
using ApiLanchonete.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Tests.Features.Orders;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_ReservesStockForBranch()
    {
        await using var context = CreateContext();
        var data = await SeedOrderScenario(context, quantity: 10);
        var service = new OrderService(context);

        var order = await service.CreateOrder(new CreateOrderDto
        {
            BranchId = data.Branch.Id,
            ClientId = data.Client.Id,
            Items = [new CreateOrderItemDto { ProductId = data.Product.Id, Quantity = 3 }]
        });

        Assert.Equal(data.Branch.Id, order.BranchId);
        Assert.Equal(30m, order.TotalAmount);
        Assert.Equal(7, await context.Inventories.Where(i => i.Id == data.Inventory.Id).Select(i => i.Quantity)
            .SingleAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateOrder_ThrowsWhenStockIsInsufficient()
    {
        await using var context = CreateContext();
        var data = await SeedOrderScenario(context, quantity: 2);
        var service = new OrderService(context);

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => service.CreateOrder(new CreateOrderDto
        {
            BranchId = data.Branch.Id,
            ClientId = data.Client.Id,
            Items = [new CreateOrderItemDto { ProductId = data.Product.Id, Quantity = 3 }]
        }));

        Assert.Contains("Insufficient stock", exception.Message);
    }

    [Fact]
    public async Task CancelOrder_ReturnsReservedStock()
    {
        await using var context = CreateContext();
        var data = await SeedOrderScenario(context, quantity: 10);
        var service = new OrderService(context);
        var order = await service.CreateOrder(new CreateOrderDto
        {
            BranchId = data.Branch.Id,
            ClientId = data.Client.Id,
            Items = [new CreateOrderItemDto { ProductId = data.Product.Id, Quantity = 4 }]
        });

        await service.UpdateOrder(order.Id, new UpdateOrderDto { Status = OrderStatus.Cancelled });

        Assert.Equal(10, await context.Inventories.Where(i => i.Id == data.Inventory.Id).Select(i => i.Quantity)
            .SingleAsync(TestContext.Current.CancellationToken));
        Assert.False(await context.Orders.Where(o => o.Id == order.Id).Select(o => o.StockReserved)
            .SingleAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateOrder_RejectsClientFromAnotherCompany()
    {
        await using var context = CreateContext();
        var data = await SeedOrderScenario(context, quantity: 10);
        var otherClient = new Client
        {
            Id = Guid.NewGuid(), CompanyId = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Outro cliente",
            Address = "Rua B", City = "São Paulo", State = "SP", CEP = "01000-001", Country = "Brasil", Phone = "11888888888"
        };
        context.Clients.Add(otherClient);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var service = new OrderService(context);

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => service.CreateOrder(new CreateOrderDto
        {
            BranchId = data.Branch.Id,
            ClientId = otherClient.Id,
            Items = [new CreateOrderItemDto { ProductId = data.Product.Id, Quantity = 1 }]
        }));

        Assert.Contains("same company", exception.Message);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<(Branch Branch, Client Client, Product Product, Inventory Inventory)> SeedOrderScenario(
        AppDbContext context,
        int quantity)
    {
        var company = new Company { Id = Guid.NewGuid(), Name = "Lanchonete", Cnpj = "12345678000190" };
        var branch = new Branch { Id = Guid.NewGuid(), CompanyId = company.Id, Name = "Centro", Active = true };
        var client = new Client
        {
            Id = Guid.NewGuid(), CompanyId = company.Id, UserId = Guid.NewGuid(), Name = "Cliente",
            Address = "Rua A", City = "São Paulo", State = "SP", CEP = "01000-000", Country = "Brasil", Phone = "11999999999"
        };
        var product = new Product
        {
            Id = Guid.NewGuid(), CompanyId = company.Id, Name = "X-Burger", Price = 10m, Active = true,
            AvailableFrom = DateTime.UtcNow.AddDays(-1)
        };
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(), BranchId = branch.Id, ProductId = product.Id, Quantity = quantity, MinimumQuantity = 1, Active = true
        };

        context.AddRange(company, branch, client, product, inventory);
        await context.SaveChangesAsync();
        return (branch, client, product, inventory);
    }
}
