using HR.Application.Common.Security;
using HR.Application.Interfaces.Authentication;

namespace HR.API.Authorization;

public static class ManagerTeamAccess
{
    public static bool HasFullAccess(System.Security.Claims.ClaimsPrincipal user) =>
        user.IsInRole(AppRoles.Admin) || user.IsInRole(AppRoles.HR);

    public static async Task<HashSet<long>> GetTeamIdsAsync(
        ICurrentUserService currentUser, IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var managerId = await currentUser.GetEmployeeIdAsync();
        return managerId is > 0
            ? (await teams.GetTeamEmployeeIdsAsync(managerId.Value, cancellationToken)).ToHashSet()
            : [];
    }
}
