using App.DAL.EF;
using App.DTO.v1.Students;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Students;

public record GetStudentListQuery(string? SearchTerm, int PageNumber = 1, int PageSize = 20) : IRequest<Result<PagedResult<StudentListItemDto>>>;

public class GetStudentListQueryHandler : IRequestHandler<GetStudentListQuery, Result<PagedResult<StudentListItemDto>>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetStudentListQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<PagedResult<StudentListItemDto>>> Handle(GetStudentListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Students
            .Where(s => s.CompanyId == _tenantContext.CompanyId);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s => 
                s.Name.ToLower().Contains(searchTerm) || 
                (s.ContactInfo != null && s.ContactInfo.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StudentListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactInfo = s.ContactInfo,
                HasAppUser = s.AppUserId != null,
                ActivePackageCount = 0, // Will be populated in Change 7
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<StudentListItemDto>(items, request.PageNumber, request.PageSize, totalCount);

        return Result<PagedResult<StudentListItemDto>>.Success(pagedResult);
    }
}
