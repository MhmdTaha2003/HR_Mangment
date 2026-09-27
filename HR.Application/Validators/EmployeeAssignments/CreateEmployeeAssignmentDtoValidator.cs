using FluentValidation;
using HR.Application.DTOs.EmployeeAssignments;

namespace HR.Application.Validators.EmployeeAssignments;
public class CreateEmployeeAssignmentDtoValidator : AbstractValidator<CreateEmployeeAssignmentDto>
{
    public CreateEmployeeAssignmentDtoValidator()
    {
        RuleFor(dto => dto.EmployeeId).GreaterThan(0);
        RuleFor(dto => dto.BranchId).GreaterThan(0);
        RuleFor(dto => dto.DepartmentId).GreaterThan(0);
        RuleFor(dto => dto.JobTitleId).GreaterThan(0);
        RuleFor(dto => dto.AssignmentType).IsInEnum();
        RuleFor(dto => dto.EndDate).GreaterThanOrEqualTo(dto => dto.StartDate).When(dto => dto.EndDate.HasValue);
        RuleFor(dto => dto.Notes).MaximumLength(1000);
    }
}

