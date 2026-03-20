using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Instructors;

public record GetCompanyUsersForDropdownQuery : IRequest<Result<List<CompanyUserDropdownDto>>>;

public class GetCompanyUsersForDropdownQueryHandler : IRequestHandler<GetCompanyUsersForDropdownQuery, Result<List<CompanyUserDropdownDto>>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetCompanyUsersForDropdownQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<List<CompanyUserDropdownDto>>> Handle(GetCompanyUsersForDropdownQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.CompanyUsers
            .Where(cu => cu.CompanyId == _tenantContext.CompanyId && cu.IsActive)
            .Select(cu => new CompanyUserDropdownDto
            {
                Id = cu.AppUserId,
                Email = cu.AppUser!.Email ?? "",
                FullName = $"{cu.AppUser.FirstName} {cu.AppUser.LastName}"
            })
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return Result<List<CompanyUserDropdownDto>>.Success(users);
    }
}