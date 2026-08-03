using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Companies;

public class CompanyService(AppDbContext context) : ICompanyService
{
    public async Task<List<CompanyDto>> GetCompanies()
        => await context.Companies
            .AsNoTracking()
            .OrderBy(company => company.Name)
            .Select(company => ToDto(company))
            .ToListAsync();

    public async Task<CompanyDto> GetCompanyById(Guid id)
    {
        var company = await context.Companies.AsNoTracking().FirstOrDefaultAsync(company => company.Id == id);
        return company is null
            ? throw new NotFoundException($"Company with ID {id} not found.")
            : ToDto(company);
    }

    public async Task<CompanyDto> CreateCompany(CreateCompanyDto dto)
    {
        var cnpj = dto.Cnpj.Trim();
        if (await context.Companies.AnyAsync(company => company.Cnpj == cnpj))
            throw new ConflictException("A company with this CNPJ already exists.");

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Cnpj = cnpj,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();
        return ToDto(company);
    }

    public async Task UpdateCompany(Guid id, UpdateCompanyDto dto)
    {
        var company = await context.Companies.FindAsync(id)
            ?? throw new NotFoundException($"Company with ID {id} not found.");

        var cnpj = dto.Cnpj.Trim();
        if (await context.Companies.AnyAsync(other => other.Id != id && other.Cnpj == cnpj))
            throw new ConflictException("A company with this CNPJ already exists.");

        company.Name = dto.Name.Trim();
        company.Cnpj = cnpj;
        company.UpdatedAt = DateTime.UtcNow;
        company.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteCompany(Guid id)
    {
        var company = await context.Companies.FindAsync(id)
            ?? throw new NotFoundException($"Company with ID {id} not found.");

        var hasDependents = await context.Branches.AnyAsync(branch => branch.CompanyId == id)
            || await context.Products.AnyAsync(product => product.CompanyId == id)
            || await context.Clients.AnyAsync(client => client.CompanyId == id);

        if (hasDependents)
            throw new ConflictException("A company with branches, products, or clients cannot be deleted.");

        context.Companies.Remove(company);
        await context.SaveChangesAsync();
    }

    private static CompanyDto ToDto(Company company) => new()
    {
        Id = company.Id,
        Name = company.Name,
        Cnpj = company.Cnpj,
        CreatedAt = company.CreatedAt,
        CreatedBy = company.CreatedBy,
        UpdatedAt = company.UpdatedAt,
        UpdatedBy = company.UpdatedBy
    };
}
