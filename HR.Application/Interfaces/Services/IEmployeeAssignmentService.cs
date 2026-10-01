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
public interface IEmployeeAssignmentService
{
    Task<List<EmployeeAssignmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<EmployeeAssignmentDto>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken);
    Task<EmployeeAssignmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeAssignmentDto> CreateAsync(CreateEmployeeAssignmentDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, UpdateEmployeeAssignmentDto dto, CancellationToken cancellationToken);
}

