using Microsoft.AspNetCore.Authorization;

namespace App.Helpers.Authorization;

public class CompanyRoleAuthorizationRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public CompanyRoleAuthorizationRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
