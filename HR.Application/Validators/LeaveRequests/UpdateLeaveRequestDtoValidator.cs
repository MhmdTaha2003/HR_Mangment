using FluentValidation;
using HR.Application.DTOs.LeaveRequests;

namespace HR.Application.Validators.LeaveRequests;
public class UpdateLeaveRequestDtoValidator : AbstractValidator<UpdateLeaveRequestDto>
{
    public UpdateLeaveRequestDtoValidator()
    {
        RuleFor(dto => dto.EndDate).GreaterThanOrEqualTo(dto => dto.StartDate);
        RuleFor(dto => dto.RequestedDays).GreaterThan(0);
        RuleFor(dto => dto.Reason).MaximumLength(1000);
        RuleFor(dto => dto.AttachmentPath).MaximumLength(500);
        RuleFor(dto => dto.Status).IsInEnum();
    }
}

