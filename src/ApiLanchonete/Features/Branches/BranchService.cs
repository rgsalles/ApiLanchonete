using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Branches;

public class BranchService(AppDbContext context) : IBranchService
{
    public async Task<List<BranchDto>> GetBranches(Guid? companyId = null)
    {
        var query = context.Branches.AsNoTracking();
        if (companyId.HasValue)
            query = query.Where(branch => branch.CompanyId == companyId.Value);

        return await query
            .OrderBy(branch => branch.Name)
            .Select(branch => ToDto(branch))
            .ToListAsync();
    }

    public async Task<BranchDto> GetBranchById(Guid id)
    {
        var branch = await context.Branches.AsNoTracking().FirstOrDefaultAsync(branch => branch.Id == id);
        return branch is null
            ? throw new NotFoundException($"Branch with ID {id} not found.")
            : ToDto(branch);
    }

    public async Task<BranchDto> CreateBranch(CreateBranchDto dto)
    {
        var companyExists = await context.Companies.AnyAsync(company => company.Id == dto.CompanyId);
        if (!companyExists)
            throw new NotFoundException($"Company with ID {dto.CompanyId} not found.");

        var name = dto.Name.Trim();
        if (await context.Branches.AnyAsync(branch => branch.CompanyId == dto.CompanyId && branch.Name == name))
            throw new ConflictException("A branch with this name already exists for the company.");

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Name = name,
            Address = dto.Address.Trim(),
            City = dto.City.Trim(),
            State = dto.State.Trim(),
            CEP = dto.CEP.Trim(),
            Country = dto.Country.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email.Trim(),
            Active = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Branches.Add(branch);
        await context.SaveChangesAsync();
        return ToDto(branch);
    }

    public async Task UpdateBranch(Guid id, UpdateBranchDto dto)
    {
        var branch = await context.Branches.FindAsync(id)
            ?? throw new NotFoundException($"Branch with ID {id} not found.");

        var name = dto.Name.Trim();
        if (await context.Branches.AnyAsync(other =>
            other.Id != id && other.CompanyId == branch.CompanyId && other.Name == name))
            throw new ConflictException("A branch with this name already exists for the company.");

        branch.Name = name;
        branch.Address = dto.Address.Trim();
        branch.City = dto.City.Trim();
        branch.State = dto.State.Trim();
        branch.CEP = dto.CEP.Trim();
        branch.Country = dto.Country.Trim();
        branch.Phone = dto.Phone.Trim();
        branch.Email = dto.Email.Trim();
        branch.Active = dto.Active;
        branch.UpdatedAt = DateTime.UtcNow;
        branch.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteBranch(Guid id)
    {
        var branch = await context.Branches.FindAsync(id)
            ?? throw new NotFoundException($"Branch with ID {id} not found.");

        var hasDependents = await context.Inventories.AnyAsync(inventory => inventory.BranchId == id)
            || await context.Orders.AnyAsync(order => order.BranchId == id);

        if (hasDependents)
            throw new ConflictException("A branch with inventory or orders cannot be deleted.");

        context.Branches.Remove(branch);
        await context.SaveChangesAsync();
    }

    private static BranchDto ToDto(Branch branch) => new()
    {
        Id = branch.Id,
        CompanyId = branch.CompanyId,
        Name = branch.Name,
        Address = branch.Address,
        City = branch.City,
        State = branch.State,
        CEP = branch.CEP,
        Country = branch.Country,
        Phone = branch.Phone,
        Email = branch.Email,
        Active = branch.Active,
        CreatedAt = branch.CreatedAt,
        CreatedBy = branch.CreatedBy,
        UpdatedAt = branch.UpdatedAt,
        UpdatedBy = branch.UpdatedBy
    };
}
