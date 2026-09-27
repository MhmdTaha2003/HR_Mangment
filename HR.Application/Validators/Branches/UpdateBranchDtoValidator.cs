using FluentValidation;
using HR.Application.DTOs.Branches;

public class UpdateBranchDtoValidator : AbstractValidator<UpdateBranchDto>
{
    public UpdateBranchDtoValidator()
    {
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.AddressEn).MaximumLength(300);
        RuleFor(dto => dto.AddressAr).MaximumLength(300);
        RuleFor(dto => dto.Phone).MaximumLength(30);
        RuleFor(dto => dto.Email).EmailAddress().MaximumLength(150).When(dto => !string.IsNullOrWhiteSpace(dto.Email));
    }
}

