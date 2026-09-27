using FluentValidation;
using HR.Application.DTOs.LeaveTypes;

namespace HR.Application.Validators.LeaveTypes;
public class CreateLeaveTypeDtoValidator : AbstractValidator<CreateLeaveTypeDto>
{
    public CreateLeaveTypeDtoValidator()
    {
        RuleFor(dto => dto.Code).NotEmpty().MaximumLength(20);
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.DefaultDays).GreaterThanOrEqualTo(0);
    }
}

