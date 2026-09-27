using FluentValidation;
using HR.Application.DTOs.EmployeeDocuments;

namespace HR.Application.Validators.EmployeeDocuments;
public class CreateEmployeeDocumentDtoValidator : AbstractValidator<CreateEmployeeDocumentDto>
{
    public CreateEmployeeDocumentDtoValidator()
    {
        RuleFor(dto => dto.EmployeeId).GreaterThan(0);
        RuleFor(dto => dto.DocumentTypeId).GreaterThan(0);
        RuleFor(dto => dto.FilePath).NotEmpty().MaximumLength(500);
        RuleFor(dto => dto.DocumentNumber).MaximumLength(100);
        RuleFor(dto => dto.ExpiryDate).GreaterThanOrEqualTo(dto => dto.IssueDate!.Value).When(dto => dto.IssueDate.HasValue && dto.ExpiryDate.HasValue);
        RuleFor(dto => dto.Notes).MaximumLength(1000);
    }
}

