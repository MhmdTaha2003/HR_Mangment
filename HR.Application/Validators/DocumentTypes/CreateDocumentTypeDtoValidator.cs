using FluentValidation;
using HR.Application.DTOs.DocumentTypes;

namespace HR.Application.Validators.DocumentTypes;
public class CreateDocumentTypeDtoValidator : AbstractValidator<CreateDocumentTypeDto>
{
    public CreateDocumentTypeDtoValidator()
    {
        RuleFor(dto => dto.Code).NotEmpty().MaximumLength(20);
        RuleFor(dto => dto.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(dto => dto.NameAr).NotEmpty().MaximumLength(150);
    }
}

