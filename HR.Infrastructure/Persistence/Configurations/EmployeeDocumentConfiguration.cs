using HR.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.Configurations;

public class EmployeeDocumentConfiguration : IEntityTypeConfiguration<EmployeeDocument>
{
    public void Configure(EntityTypeBuilder<EmployeeDocument> builder)
    {
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.DocumentNumber).HasMaxLength(100);
        builder.Property(x => x.FilePath).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Employee).WithMany(x => x.Documents)
            .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DocumentType).WithMany(x => x.EmployeeDocuments)
            .HasForeignKey(x => x.DocumentTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}
