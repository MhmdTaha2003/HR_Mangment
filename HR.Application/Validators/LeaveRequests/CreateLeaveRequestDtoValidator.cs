using FluentValidation;
using HR.Application.DTOs.LeaveRequests;

namespace HR.Application.Validators.LeaveRequests;
public class CreateLeaveRequestDtoValidator : AbstractValidator<CreateLeaveRequestDto>
{
    public CreateLeaveRequestDtoValidator()
    {
        RuleFor(dto => dto.EmployeeId).GreaterThan(0);
        RuleFor(dto => dto.LeaveTypeId).GreaterThan(0);
        RuleFor(dto => dto.EndDate).GreaterThanOrEqualTo(dto => dto.StartDate);
        RuleFor(dto => dto.RequestedDays).GreaterThan(0);
        RuleFor(dto => dto.Reason).MaximumLength(1000);
        RuleFor(dto => dto.AttachmentPath).MaximumLength(500);
    }
}

