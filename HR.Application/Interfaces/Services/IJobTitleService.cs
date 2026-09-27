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
public interface IJobTitleService
{
    Task<List<JobTitleDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<JobTitleDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<JobTitleDto> CreateAsync(CreateJobTitleDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, UpdateJobTitleDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}

