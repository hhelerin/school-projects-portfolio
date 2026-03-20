using Microsoft.AspNetCore.Authorization;

namespace App.Helpers.Authorization;

public class CompanyRoleAuthorizeAttribute : AuthorizeAttribute
{
    public CompanyRoleAuthorizeAttribute(params string[] roles)
    {
        Policy = $"CompanyRoles:{string.Join(",", roles)}";
    }
}
