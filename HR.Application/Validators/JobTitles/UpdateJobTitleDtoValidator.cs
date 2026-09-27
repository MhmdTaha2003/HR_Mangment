using FluentValidation;
using HR.Application.DTOs.JobTitles;

namespace HR.Application.Validators.JobTitles;
public class UpdateJobTitleDtoValidator : AbstractValidator<UpdateJobTitleDto>
{
    public UpdateJobTitleDtoValidator()
    {
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.Description).MaximumLength(500);
    }
}

