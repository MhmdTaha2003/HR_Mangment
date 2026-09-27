using FluentValidation;
using HR.Application.DTOs.Employees;

namespace HR.Application.Validators.Employees;
public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(dto => dto.FirstNameEn).NotEmpty().MaximumLength(100);
        RuleFor(dto => dto.MiddleNameEn).MaximumLength(100);
        RuleFor(dto => dto.LastNameEn).NotEmpty().MaximumLength(100);
        RuleFor(dto => dto.FirstNameAr).NotEmpty().MaximumLength(100);
        RuleFor(dto => dto.MiddleNameAr).MaximumLength(100);
        RuleFor(dto => dto.LastNameAr).NotEmpty().MaximumLength(100);
        RuleFor(dto => dto.Email).EmailAddress().MaximumLength(256).When(dto => !string.IsNullOrWhiteSpace(dto.Email));
        RuleFor(dto => dto.PhoneNumber).MaximumLength(30);
        RuleFor(dto => dto.DateOfBirth).LessThan(DateTime.UtcNow).When(dto => dto.DateOfBirth.HasValue);
        RuleFor(dto => dto.HireDate).NotEmpty().Must((dto, value) => !dto.DateOfBirth.HasValue || value >= dto.DateOfBirth.Value).WithMessage("Hire date cannot precede date of birth.");
        RuleFor(dto => dto.TerminationDate).Must((dto, value) => !value.HasValue || value.Value >= DateOnly.FromDateTime(dto.HireDate)).WithMessage("Termination date cannot precede hire date.");
        RuleFor(dto => dto.TerminationDate).NotNull().When(dto => dto.EmploymentStatus is HR.Domain.Enums.EmploymentStatus.Terminated or HR.Domain.Enums.EmploymentStatus.Resigned);
        RuleFor(dto => dto.TerminationDate).Null().When(dto => dto.EmploymentStatus == HR.Domain.Enums.EmploymentStatus.Active);
        RuleFor(dto => dto.Gender).IsInEnum();
        RuleFor(dto => dto.EmploymentStatus).IsInEnum();
    }
}

