using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.CommonLibrary.Security
{
    public static class AuthorizationPolicies
    {
        public const string RequireAdminRole = "RequireAdminRole";
        public const string RequireManagerRole = "RequireManagerRole";
        public const string RequireEmployeeRole = "RequireEmployeeRole";

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(RequireAdminRole, policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy(RequireManagerRole, policy =>
                    policy.RequireRole("Manager"));

                options.AddPolicy(RequireEmployeeRole, policy =>
                    policy.RequireRole("Employee"));

                options.AddPolicy("RequireApiScope", policy =>
                    policy.RequireAuthenticatedUser()
                         .RequireClaim("scope", "api1"));

                options.AddPolicy("CanManageEmployees", policy =>
                    policy.RequireRole("Admin", "Manager")
                         .RequireClaim("permission", "manage:employees"));
            });
        }
    }
}