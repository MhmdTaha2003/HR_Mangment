using FluentValidation;
using HR.Application.DTOs.LeaveRequestApprovals;

namespace HR.Application.Validators.LeaveRequestApprovals;
public class UpdateLeaveRequestApprovalDtoValidator : AbstractValidator<UpdateLeaveRequestApprovalDto>
{
    public UpdateLeaveRequestApprovalDtoValidator()
    {
        RuleFor(dto => dto.Action).IsInEnum();
        RuleFor(dto => dto.Comments).MaximumLength(1000);
    }
}

