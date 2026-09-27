using FluentValidation;
using HR.Application.DTOs.EmployeeLeaveBalances;

namespace HR.Application.Validators.EmployeeLeaveBalances;
public class CreateEmployeeLeaveBalanceDtoValidator : AbstractValidator<CreateEmployeeLeaveBalanceDto>
{
    public CreateEmployeeLeaveBalanceDtoValidator()
    {
        RuleFor(dto => dto.EmployeeId).GreaterThan(0);
        RuleFor(dto => dto.LeaveTypeId).GreaterThan(0);
        RuleFor(dto => dto.Year).InclusiveBetween(1900, 2100);
        RuleFor(dto => dto.EntitledDays).GreaterThanOrEqualTo(0);
        RuleFor(dto => dto.CarriedForwardDays).GreaterThanOrEqualTo(0);
        RuleFor(dto => dto.UsedDays).GreaterThanOrEqualTo(0);
    }
}

