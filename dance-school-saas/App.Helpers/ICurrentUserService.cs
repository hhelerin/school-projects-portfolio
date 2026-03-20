namespace App.Helpers;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid CompanyId { get; }
    string[] Roles { get; }
    Guid? GetCurrentUserId();
}