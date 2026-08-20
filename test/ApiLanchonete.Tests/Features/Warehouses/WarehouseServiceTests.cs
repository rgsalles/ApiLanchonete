using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using ApiLanchonete.Features.Branches;
using ApiLanchonete.Features.Companies;
using ApiLanchonete.Features.Products;
using ApiLanchonete.Features.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Tests.Features.Warehouses;

public class WarehouseServiceTests
{
    [Fact]
    public async Task CreateWarehouse_RegistersStockForBranchProduct()
    {
        await using var context = TestDbContextFactory.Create();
        var company = new Company { Id = Guid.NewGuid(), Name = "Lanchonete", Cnpj = "12345678000190" };
        var branch = new Branch { Id = Guid.NewGuid(), CompanyId = company.Id, Name = "Centro" };
        var product = new Product { Id = Guid.NewGuid(), CompanyId = company.Id, Name = "X-Burger", Price = 10m, Active = true, AvailableFrom = DateTime.UtcNow.AddDays(-1) };

        context.Companies.Add(company);
        context.Branches.Add(branch);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new WarehouseService(context);
        var warehouse = await service.CreateWarehouse(new CreateWarehouseDto
        {
            BranchId = branch.Id,
            ProductId = product.Id,
            Quantity = 25,
            MinimumQuantity = 5
        });

        Assert.Equal(branch.Id, warehouse.BranchId);
        Assert.Equal(product.Id, warehouse.ProductId);
        Assert.Equal(25, warehouse.Quantity);
        Assert.Equal("System", warehouse.CreatedBy);
    }

    [Fact]
    public async Task CreateWarehouse_RejectsDuplicateProductInSameBranch()
    {
        await using var context = TestDbContextFactory.Create();
        var company = new Company { Id = Guid.NewGuid(), Name = "Lanchonete", Cnpj = "12345678000190" };
        var branch = new Branch { Id = Guid.NewGuid(), CompanyId = company.Id, Name = "Centro" };
        var product = new Product { Id = Guid.NewGuid(), CompanyId = company.Id, Name = "X-Burger", Price = 10m, Active = true, AvailableFrom = DateTime.UtcNow.AddDays(-1) };

        context.Companies.Add(company);
        context.Branches.Add(branch);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new WarehouseService(context);
        await service.CreateWarehouse(new CreateWarehouseDto
        {
            BranchId = branch.Id,
            ProductId = product.Id,
            Quantity = 10,
            MinimumQuantity = 2
        });

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateWarehouse(new CreateWarehouseDto
        {
            BranchId = branch.Id,
            ProductId = product.Id,
            Quantity = 5,
            MinimumQuantity = 1
        }));
    }

    [Fact]
    public async Task CreateWarehouse_RejectsProductFromAnotherCompany()
    {
        await using var context = TestDbContextFactory.Create();
        var companyA = new Company { Id = Guid.NewGuid(), Name = "Lanchonete A", Cnpj = "12345678000191" };
        var companyB = new Company { Id = Guid.NewGuid(), Name = "Lanchonete B", Cnpj = "12345678000192" };
        var branch = new Branch { Id = Guid.NewGuid(), CompanyId = companyA.Id, Name = "Centro" };
        var product = new Product { Id = Guid.NewGuid(), CompanyId = companyB.Id, Name = "X-Burger", Price = 10m, Active = true, AvailableFrom = DateTime.UtcNow.AddDays(-1) };

        context.Companies.AddRange(companyA, companyB);
        context.Branches.Add(branch);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new WarehouseService(context);

        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateWarehouse(new CreateWarehouseDto
        {
            BranchId = branch.Id,
            ProductId = product.Id,
            Quantity = 15,
            MinimumQuantity = 3
        }));
    }
}