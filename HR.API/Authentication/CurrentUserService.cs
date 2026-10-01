using System.Security.Claims;
using HR.Application.Interfaces.Authentication;
using HR.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace HR.API.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<long?> GetEmployeeIdAsync()
    {
        if (string.IsNullOrWhiteSpace(UserId))
            return null;

        var user = await _userManager.FindByIdAsync(UserId);

        return user?.EmployeeId;
    }
}