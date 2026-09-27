using FluentValidation;
using HR.Application.DTOs.Departments;

namespace HR.Application.Validators.Departments;
public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(dto => dto.Code).NotEmpty().MaximumLength(20);
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.BranchId).GreaterThan(0);
    }
}

