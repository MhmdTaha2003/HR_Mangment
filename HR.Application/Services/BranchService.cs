using HR.Application.DTOs.Branches;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class BranchService : IBranchService
{
    private readonly IBranchRepository _branchRepository;
    public BranchService(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task<List<BranchDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        return branches.Select(Map).ToList();
    }

    public async Task<BranchDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var branch = await _branchRepository.GetByIdAsync(id, cancellationToken);
        return branch is null ? null : Map(branch);
    }

    public async Task<BranchDto> CreateAsync(CreateBranchDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        if (await _branchRepository.CodeExistsAsync(normalizedCode, cancellationToken))
        {
            throw new InvalidOperationException("Branch code already exists.");
        }

        var branch = new Branch
        {
            Code = normalizedCode,
            NameEn = dto.NameEn.Trim(),
            NameAr = dto.NameAr.Trim(),
            AddressEn = dto.AddressEn?.Trim(),
            AddressAr = dto.AddressAr?.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim(),
            IsActive = true
        };
        await _branchRepository.AddAsync(branch, cancellationToken);
        await _branchRepository.SaveChangesAsync(cancellationToken);
        return Map(branch);
    }

    public async Task<bool> UpdateAsync(long id, UpdateBranchDto dto, CancellationToken cancellationToken = default)
    {
        var branch = await _branchRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (branch is null)
            return false;
        branch.NameEn = dto.NameEn.Trim();
        branch.NameAr = dto.NameAr.Trim();
        branch.AddressEn = dto.AddressEn?.Trim();
        branch.AddressAr = dto.AddressAr?.Trim();
        branch.Phone = dto.Phone?.Trim();
        branch.Email = dto.Email?.Trim();
        branch.IsActive = dto.IsActive;
        branch.UpdatedAt = DateTime.UtcNow;
        await _branchRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var branch = await _branchRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (branch is null)
            return false;
        branch.IsActive = false;
        branch.UpdatedAt = DateTime.UtcNow;
        await _branchRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static BranchDto Map(Branch branch)
    {
        return new BranchDto
        {
            Id = branch.Id,
            Code = branch.Code,
            NameEn = branch.NameEn,
            NameAr = branch.NameAr,
            AddressEn = branch.AddressEn,
            AddressAr = branch.AddressAr,
            Phone = branch.Phone,
            Email = branch.Email,
            IsActive = branch.IsActive
        };
    }
}
