using HR.Domain.Enums;

namespace HR.Application.DTOs.Employees;
public record CreateEmployeeDto(string EmployeeNumber, string FirstNameEn, string? MiddleNameEn, string LastNameEn, string FirstNameAr, string? MiddleNameAr, string LastNameAr, string? Email, string? PhoneNumber, DateTime? DateOfBirth, DateTime HireDate, DateOnly? TerminationDate, Gender Gender, EmploymentStatus EmploymentStatus);
