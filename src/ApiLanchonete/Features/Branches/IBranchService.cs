namespace ApiLanchonete.Features.Branches;

public interface IBranchService
{
    Task<List<BranchDto>> GetBranches(Guid? companyId = null);
    Task<BranchDto> GetBranchById(Guid id);
    Task<BranchDto> CreateBranch(CreateBranchDto dto);
    Task UpdateBranch(Guid id, UpdateBranchDto dto);
    Task DeleteBranch(Guid id);
}
