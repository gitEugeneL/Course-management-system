using System.Security.Claims;
using API.Data.Enums;
using Api.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Api.Utils;

public static class AuthPolicy
{
    public static void ConfigureAuthPolicy(IServiceCollection service)
    {
        var commonPolicy = new AuthorizationPolicyBuilder()
            .RequireClaim(ClaimTypes.Email)
            .RequireClaim(ClaimTypes.NameIdentifier)
            .Build();

        service.AddAuthorizationBuilder()
            .AddPolicy(AppConstants.BasePolicy, commonPolicy)

            .AddPolicy(AppConstants.ProfessorPolicy, policy =>
                policy
                    .RequireRole(Role.Professor.ToString())
                    .AddRequirements(commonPolicy.Requirements.ToArray()))

            .AddPolicy(AppConstants.StudentPolicy, policy =>
                policy
                    .RequireRole(Role.Student.ToString())
                    .AddRequirements(commonPolicy.Requirements.ToArray()));
    }
}