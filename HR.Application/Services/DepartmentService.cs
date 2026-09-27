using HR.Application.DTOs.Departments;
using HR.Application.DTOs.JobTitles;
using HR.Application.DTOs.DocumentTypes;
using HR.Application.DTOs.LeaveTypes;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IBranchRepository _branchRepository;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IBranchRepository branchRepository)
    {
        _departmentRepository = departmentRepository;
        _branchRepository = branchRepository;
    }

    public async Task<List<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken) => (await _departmentRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<DepartmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        return department is null ? null : Map(department);
    }

    async Task Validate(long branchId, long? parentId, long? self, CancellationToken cancellationToken)
    {
        if (await _branchRepository.GetByIdAsync(branchId, cancellationToken) is null)
            throw new InvalidOperationException("Active branch not found.");
        var visited = new HashSet<long>();
        while (parentId.HasValue)
        {
            if (parentId == self)
                throw new InvalidOperationException("The selected parent would create a department hierarchy cycle.");
            if (!visited.Add(parentId.Value))
                throw new InvalidOperationException("The existing department hierarchy contains a cycle.");
            var parentDepartment = await _departmentRepository.GetByIdAsync(parentId.Value, cancellationToken) ?? throw new InvalidOperationException("Active parent department not found.");
            if (parentDepartment.BranchId != branchId)
                throw new InvalidOperationException("Parent department must belong to the selected branch.");
            parentId = parentDepartment.ParentDepartmentId;
        }
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var code = dto.Code.Trim().ToUpperInvariant();
        if (await _departmentRepository.CodeExistsAsync(code, cancellationToken))
            throw new InvalidOperationException("Department code already exists.");
        await Validate(dto.BranchId, dto.ParentDepartmentId, null, cancellationToken);
        var department = new Department
        {
            Code = code,
            NameEn = dto.NameEn.Trim(),
            NameAr = dto.NameAr.Trim(),
            BranchId = dto.BranchId,
            ParentDepartmentId = dto.ParentDepartmentId,
            IsActive = true
        };
        await _departmentRepository.AddAsync(department, cancellationToken);
        await _departmentRepository.SaveChangesAsync(cancellationToken);
        return Map(department);
    }

    public async Task<bool> UpdateAsync(long id, UpdateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (department is null)
            return false;
        await Validate(dto.BranchId, dto.ParentDepartmentId, id, cancellationToken);
        department.NameEn = dto.NameEn.Trim();
        department.NameAr = dto.NameAr.Trim();
        department.BranchId = dto.BranchId;
        department.ParentDepartmentId = dto.ParentDepartmentId;
        department.IsActive = dto.IsActive;
        department.UpdatedAt = DateTime.UtcNow;
        await _departmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (department is null)
            return false;
        department.IsActive = false;
        department.UpdatedAt = DateTime.UtcNow;
        await _departmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static DepartmentDto Map(Department department) => new(department.Id, department.Code, department.NameEn, department.NameAr, department.BranchId, department.ParentDepartmentId, department.IsActive);
}

