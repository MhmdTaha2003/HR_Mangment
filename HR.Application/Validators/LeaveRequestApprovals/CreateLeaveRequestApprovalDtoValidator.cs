using FluentValidation;
using HR.Application.DTOs.LeaveRequestApprovals;

namespace HR.Application.Validators.LeaveRequestApprovals;
public class CreateLeaveRequestApprovalDtoValidator : AbstractValidator<CreateLeaveRequestApprovalDto>
{
    public CreateLeaveRequestApprovalDtoValidator()
    {
        RuleFor(dto => dto.LeaveRequestId).GreaterThan(0);
        RuleFor(dto => dto.ApprovalLevel).GreaterThan(0);
        RuleFor(dto => dto.ApproverEmployeeId).GreaterThan(0);
        RuleFor(dto => dto.Action).IsInEnum();
        RuleFor(dto => dto.Comments).MaximumLength(1000);
    }
}

