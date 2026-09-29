using HR.Application.Common.Security;

using Microsoft.AspNetCore.Authorization;

namespace HR.API.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAppAuthorization(
        this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(AppPolicies.AdminOnly, policy =>
                    policy.RequireRole(AppRoles.Admin));

                options.AddPolicy(AppPolicies.HRManagement, policy =>
                    policy.RequireRole(
                        AppRoles.Admin,
                        AppRoles.HR));

                options.AddPolicy(AppPolicies.LeaveApproval, policy =>
                    policy.RequireRole(
                        AppRoles.Admin,
                        AppRoles.HR,
                        AppRoles.Manager));
            });

            return services;
        }

    }
}
