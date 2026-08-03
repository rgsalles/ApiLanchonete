using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Features.Companies;

namespace ApiLanchonete.Tests.Features.Companies;

public class CompanyServiceTests
{
    [Fact]
    public async Task CreateCompany_TrimsValuesAndSetsAuditData()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new CompanyService(context);

        var company = await service.CreateCompany(new CreateCompanyDto
        {
            Name = "  Lanchonete Central  ", Cnpj = " 12345678000190 "
        });

        Assert.Equal("Lanchonete Central", company.Name);
        Assert.Equal("12345678000190", company.Cnpj);
        Assert.Equal("System", company.CreatedBy);
    }

    [Fact]
    public async Task CreateCompany_RejectsDuplicateCnpj()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new CompanyService(context);
        var dto = new CreateCompanyDto { Name = "Lanchonete", Cnpj = "12345678000190" };
        await service.CreateCompany(dto);

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateCompany(new CreateCompanyDto
        {
            Name = "Outra", Cnpj = dto.Cnpj
        }));
    }
}
