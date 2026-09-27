using FluentValidation;
using HR.Application.DTOs.EmployeeContracts;

namespace HR.Application.Validators.EmployeeContracts;
public class CreateEmployeeContractDtoValidator : AbstractValidator<CreateEmployeeContractDto>
{
    public CreateEmployeeContractDtoValidator()
    {
        RuleFor(dto => dto.EmployeeId).GreaterThan(0);
        RuleFor(dto => dto.ContractNumber).NotEmpty().MaximumLength(50);
        RuleFor(dto => dto.ContractType).IsInEnum();
        RuleFor(dto => dto.Status).IsInEnum();
        RuleFor(dto => dto.EndDate).GreaterThanOrEqualTo(dto => dto.StartDate).When(dto => dto.EndDate.HasValue);
        RuleFor(dto => dto.ProbationEndDate).Must((dto, value) => !value.HasValue || value.Value >= DateOnly.FromDateTime(dto.StartDate)).WithMessage("Probation end date cannot precede start date.").Must((dto, value) => !value.HasValue || !dto.EndDate.HasValue || value.Value <= DateOnly.FromDateTime(dto.EndDate.Value)).WithMessage("Probation end date cannot follow end date.");
        RuleFor(dto => dto.Salary).GreaterThanOrEqualTo(0).When(dto => dto.Salary.HasValue);
        RuleFor(dto => dto.Notes).MaximumLength(1000);
    }
}

