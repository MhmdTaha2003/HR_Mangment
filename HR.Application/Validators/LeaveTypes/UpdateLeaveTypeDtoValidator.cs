using FluentValidation;
using HR.Application.DTOs.LeaveTypes;

namespace HR.Application.Validators.LeaveTypes;
public class UpdateLeaveTypeDtoValidator : AbstractValidator<UpdateLeaveTypeDto>
{
    public UpdateLeaveTypeDtoValidator()
    {
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.DefaultDays).GreaterThanOrEqualTo(0);
    }
}

