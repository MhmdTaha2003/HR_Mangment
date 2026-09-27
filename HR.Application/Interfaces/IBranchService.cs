using HR.Application.DTOs.Branches;

namespace HR.Application.Interfaces.Services;
public interface IBranchService
{
    Task<List<BranchDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BranchDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BranchDto> CreateAsync(CreateBranchDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(long id, UpdateBranchDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
