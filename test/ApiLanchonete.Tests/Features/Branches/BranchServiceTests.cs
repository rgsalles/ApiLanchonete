using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Features.Branches;
using ApiLanchonete.Features.Companies;

namespace ApiLanchonete.Tests.Features.Branches;

public class BranchServiceTests
{
    [Fact]
    public async Task CreateBranch_RejectsUnknownCompany()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new BranchService(context);

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateBranch(CreateBranchDto(Guid.NewGuid(), "Centro")));
    }

    [Fact]
    public async Task CreateBranch_RejectsDuplicateNameWithinCompany()
    {
        await using var context = TestDbContextFactory.Create();
        var company = new Company { Id = Guid.NewGuid(), Name = "Lanchonete", Cnpj = "12345678000190" };
        context.Companies.Add(company);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var service = new BranchService(context);

        await service.CreateBranch(CreateBranchDto(company.Id, "Centro"));

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateBranch(CreateBranchDto(company.Id, "Centro")));
    }

    private static CreateBranchDto CreateBranchDto(Guid companyId, string name) => new()
    {
        CompanyId = companyId,
        Name = name,
        Address = "Rua A, 1",
        City = "São Paulo",
        State = "SP",
        CEP = "01000-000",
        Country = "Brasil",
        Phone = "11999999999",
        Email = "centro@lanchonete.com"
    };
}
