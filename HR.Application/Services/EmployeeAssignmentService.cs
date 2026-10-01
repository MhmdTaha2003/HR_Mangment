using HR.Application.DTOs.Employees;
using HR.Application.DTOs.EmployeeAssignments;
using HR.Application.DTOs.EmployeeContracts;
using HR.Application.DTOs.EmployeeDocuments;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class EmployeeAssignmentService : IEmployeeAssignmentService
{
    private readonly IEmployeeAssignmentRepository _employeeAssignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IJobTitleRepository _jobTitleRepository;

    public EmployeeAssignmentService(
        IEmployeeAssignmentRepository employeeAssignmentRepository,
        IEmployeeRepository employeeRepository,
        IBranchRepository branchRepository,
        IDepartmentRepository departmentRepository,
        IJobTitleRepository jobTitleRepository)
    {
        _employeeAssignmentRepository = employeeAssignmentRepository;
        _employeeRepository = employeeRepository;
        _branchRepository = branchRepository;
        _departmentRepository = departmentRepository;
        _jobTitleRepository = jobTitleRepository;
    }

    public async Task<List<EmployeeAssignmentDto>> GetAllAsync(CancellationToken cancellationToken) => (await _employeeAssignmentRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<List<EmployeeAssignmentDto>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken) => (await _employeeAssignmentRepository.GetByEmployeeIdAsync(employeeId, cancellationToken)).Select(Map).ToList();
    public async Task<EmployeeAssignmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var assignment = await _employeeAssignmentRepository.GetByIdAsync(id, cancellationToken);
        return assignment is null ? null : Map(assignment);
    }

    async Task Check(long emp, long branch, long dept, long job, long? manager, bool current, long? except, CancellationToken cancellationToken)
    {
        if (!await _employeeRepository.ExistsAsync(emp, cancellationToken))
            throw new InvalidOperationException("Employee not found.");
        if (await _branchRepository.GetByIdAsync(branch, cancellationToken) is null)
            throw new InvalidOperationException("Active branch not found.");
        var dto = await _departmentRepository.GetByIdAsync(dept, cancellationToken) ?? throw new InvalidOperationException("Active department not found.");
        if (dto.BranchId != branch)
            throw new InvalidOperationException("Department does not belong to the selected branch.");
        if (await _jobTitleRepository.GetByIdAsync(job, cancellationToken) is null)
            throw new InvalidOperationException("Active job title not found.");
        if (manager.HasValue && !await _employeeRepository.ExistsAsync(manager.Value, cancellationToken))
            throw new InvalidOperationException("Manager employee not found.");
        if (manager == emp)
            throw new InvalidOperationException("Employee cannot manage themselves.");
        if (current && await _employeeAssignmentRepository.HasOtherCurrentAsync(emp, except, cancellationToken))
            throw new InvalidOperationException("Employee already has a current assignment.");
    }

    public async Task<EmployeeAssignmentDto> CreateAsync(CreateEmployeeAssignmentDto dto, CancellationToken cancellationToken)
    {
        await Check(dto.EmployeeId, dto.BranchId, dto.DepartmentId, dto.JobTitleId, dto.ManagerEmployeeId, dto.IsCurrent, null, cancellationToken);
        var assignment = new EmployeeAssignment
        {
            EmployeeId = dto.EmployeeId,
            BranchId = dto.BranchId,
            DepartmentId = dto.DepartmentId,
            JobTitleId = dto.JobTitleId,
            ManagerEmployeeId = dto.ManagerEmployeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            AssignmentType = dto.AssignmentType,
            IsCurrent = dto.IsCurrent,
            Notes = dto.Notes?.Trim()
        };
        await _employeeAssignmentRepository.AddAsync(assignment, cancellationToken);
        await _employeeAssignmentRepository.SaveChangesAsync(cancellationToken);
        return Map(assignment);
    }

    public async Task<bool> UpdateAsync(long id, UpdateEmployeeAssignmentDto dto, CancellationToken cancellationToken)
    {
        var assignment = await _employeeAssignmentRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (assignment is null)
            return false;
        await Check(assignment.EmployeeId, dto.BranchId, dto.DepartmentId, dto.JobTitleId, dto.ManagerEmployeeId, dto.IsCurrent, id, cancellationToken);
        assignment.BranchId = dto.BranchId;
        assignment.DepartmentId = dto.DepartmentId;
        assignment.JobTitleId = dto.JobTitleId;
        assignment.ManagerEmployeeId = dto.ManagerEmployeeId;
        assignment.StartDate = dto.StartDate;
        assignment.EndDate = dto.EndDate;
        assignment.AssignmentType = dto.AssignmentType;
        assignment.IsCurrent = dto.IsCurrent;
        assignment.Notes = dto.Notes?.Trim();
        assignment.UpdatedAt = DateTime.UtcNow;
        await _employeeAssignmentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static EmployeeAssignmentDto Map(EmployeeAssignment assignment) => new(assignment.Id, assignment.EmployeeId, assignment.BranchId, assignment.DepartmentId, assignment.JobTitleId, assignment.ManagerEmployeeId, assignment.StartDate, assignment.EndDate, assignment.AssignmentType, assignment.IsCurrent, assignment.Notes);
}

