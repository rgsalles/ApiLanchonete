namespace ApiLanchonete.Features.Companies;

public interface ICompanyService
{
    Task<List<CompanyDto>> GetCompanies();
    Task<CompanyDto> GetCompanyById(Guid id);
    Task<CompanyDto> CreateCompany(CreateCompanyDto dto);
    Task UpdateCompany(Guid id, UpdateCompanyDto dto);
    Task DeleteCompany(Guid id);
}
