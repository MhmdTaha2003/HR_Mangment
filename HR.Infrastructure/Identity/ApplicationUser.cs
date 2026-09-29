using HR.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace HR.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public long? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}