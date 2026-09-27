using FluentValidation;
using HR.Application.DTOs.EmployeeContracts;

namespace HR.Application.Validators.EmployeeContracts;
public class UpdateEmployeeContractDtoValidator : AbstractValidator<UpdateEmployeeContractDto>
{
    public UpdateEmployeeContractDtoValidator()
    {
        RuleFor(dto => dto.ContractType).IsInEnum();
        RuleFor(dto => dto.Status).IsInEnum();
        RuleFor(dto => dto.EndDate).GreaterThanOrEqualTo(dto => dto.StartDate).When(dto => dto.EndDate.HasValue);
        RuleFor(dto => dto.ProbationEndDate).Must((dto, value) => !value.HasValue || value.Value >= DateOnly.FromDateTime(dto.StartDate)).Must((dto, value) => !value.HasValue || !dto.EndDate.HasValue || value.Value <= DateOnly.FromDateTime(dto.EndDate.Value));
        RuleFor(dto => dto.Salary).GreaterThanOrEqualTo(0).When(dto => dto.Salary.HasValue);
        RuleFor(dto => dto.Notes).MaximumLength(1000);
    }
}

