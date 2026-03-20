using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Instructors;

public record GetListInstructorQuery : IRequest<Result<List<InstructorListItemDto>>>;

public class GetListInstructorQueryHandler : IRequestHandler<GetListInstructorQuery, Result<List<InstructorListItemDto>>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetListInstructorQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<List<InstructorListItemDto>>> Handle(GetListInstructorQuery request, CancellationToken cancellationToken)
    {
        var instructors = await _context.Instructors
            .Where(i => i.CompanyId == _tenantContext.CompanyId)
            .OrderBy(i => i.Name)
            .Select(i => new InstructorListItemDto
            {
                Id = i.Id,
                Name = i.Name,
                AppUserId = i.AppUserId
            })
            .ToListAsync(cancellationToken);

        return Result<List<InstructorListItemDto>>.Success(instructors);
    }
}