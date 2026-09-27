using HR.Application.DTOs.Departments;
using HR.Application.DTOs.JobTitles;
using HR.Application.DTOs.Employees;
using HR.Application.DTOs.EmployeeAssignments;
using HR.Application.DTOs.EmployeeContracts;
using HR.Application.DTOs.DocumentTypes;
using HR.Application.DTOs.EmployeeDocuments;
using HR.Application.DTOs.LeaveTypes;
using HR.Application.DTOs.EmployeeLeaveBalances;
using HR.Application.DTOs.LeaveRequests;
using HR.Application.DTOs.LeaveRequestApprovals;

namespace HR.Application.Interfaces.Services;
public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DepartmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, UpdateDepartmentDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}

