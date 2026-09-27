using FluentValidation;
using HR.Application.DTOs.DocumentTypes;

namespace HR.Application.Validators.DocumentTypes;
public class UpdateDocumentTypeDtoValidator : AbstractValidator<UpdateDocumentTypeDto>
{
    public UpdateDocumentTypeDtoValidator()
    {
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
    }
}

