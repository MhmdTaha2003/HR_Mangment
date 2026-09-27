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
public interface IEmployeeDocumentService
{
    Task<List<EmployeeDocumentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<EmployeeDocumentDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeDocumentDto> CreateAsync(CreateEmployeeDocumentDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, UpdateEmployeeDocumentDto dto, CancellationToken cancellationToken);
}

