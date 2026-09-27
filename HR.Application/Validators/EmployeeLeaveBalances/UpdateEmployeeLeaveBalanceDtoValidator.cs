using FluentValidation;
using HR.Application.DTOs.EmployeeLeaveBalances;

namespace HR.Application.Validators.EmployeeLeaveBalances;
public class UpdateEmployeeLeaveBalanceDtoValidator : AbstractValidator<UpdateEmployeeLeaveBalanceDto>
{
    public UpdateEmployeeLeaveBalanceDtoValidator()
    {
        RuleFor(dto => dto.EntitledDays).GreaterThanOrEqualTo(0);
        RuleFor(dto => dto.CarriedForwardDays).GreaterThanOrEqualTo(0);
        RuleFor(dto => dto.UsedDays).GreaterThanOrEqualTo(0);
    }
}

