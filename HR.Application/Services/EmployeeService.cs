using HR.Application.DTOs.Employees;
using HR.Application.DTOs.EmployeeAssignments;
using HR.Application.DTOs.EmployeeContracts;
using HR.Application.DTOs.EmployeeDocuments;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken) => (await _employeeRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<EmployeeDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : Map(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var normalizedEmployeeNumber = dto.EmployeeNumber.Trim().ToUpperInvariant();
        if (await _employeeRepository.NumberExistsAsync(normalizedEmployeeNumber, cancellationToken))
            throw new InvalidOperationException("Employee number already exists.");
        var employee = new Employee
        {
            EmployeeNumber = normalizedEmployeeNumber
        };
        Set(employee, dto.FirstNameEn, dto.MiddleNameEn, dto.LastNameEn, dto.FirstNameAr, dto.MiddleNameAr, dto.LastNameAr, dto.Email, dto.PhoneNumber, dto.DateOfBirth, dto.HireDate, dto.TerminationDate, dto.Gender, dto.EmploymentStatus);
        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Map(employee);
    }

    public async Task<bool> UpdateAsync(long id, UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (employee is null)
            return false;
        Set(employee, dto.FirstNameEn, dto.MiddleNameEn, dto.LastNameEn, dto.FirstNameAr, dto.MiddleNameAr, dto.LastNameAr, dto.Email, dto.PhoneNumber, dto.DateOfBirth, dto.HireDate, dto.TerminationDate, dto.Gender, dto.EmploymentStatus);
        employee.UpdatedAt = DateTime.UtcNow;
        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static void Set(Employee employee, string firstNameEn, string? middleNameEn, string lastNameEn, string firstNameAr, string? middleNameAr, string lastNameAr, string? email, string? phone, DateTime? dateOfBirth, DateTime hireDate, DateOnly? terminationDate, HR.Domain.Enums.Gender gender, HR.Domain.Enums.EmploymentStatus status)
    {
        employee.FirstNameEn = firstNameEn.Trim();
        employee.MiddleNameEn = middleNameEn?.Trim();
        employee.LastNameEn = lastNameEn.Trim();
        employee.FirstNameAr = firstNameAr.Trim();
        employee.MiddleNameAr = middleNameAr?.Trim();
        employee.LastNameAr = lastNameAr.Trim();
        employee.Email = email?.Trim();
        employee.PhoneNumber = phone?.Trim();
        employee.DateOfBirth = dateOfBirth;
        employee.HireDate = hireDate;
        employee.TerminationDate = terminationDate;
        employee.Gender = gender;
        employee.EmploymentStatus = status;
    }

    static EmployeeDto Map(Employee employee) => new(employee.Id, employee.EmployeeNumber, employee.FirstNameEn, employee.MiddleNameEn, employee.LastNameEn, employee.FirstNameAr, employee.MiddleNameAr, employee.LastNameAr, employee.Email, employee.PhoneNumber, employee.DateOfBirth, employee.HireDate, employee.TerminationDate, employee.Gender, employee.EmploymentStatus);
}

