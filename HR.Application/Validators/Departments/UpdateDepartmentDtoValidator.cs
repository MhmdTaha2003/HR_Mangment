using FluentValidation;
using HR.Application.DTOs.Departments;

namespace HR.Application.Validators.Departments;
public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentDtoValidator()
    {
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.BranchId).GreaterThan(0);
    }
}

