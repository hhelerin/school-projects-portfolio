using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Studios;

public record GetStudioByIdQuery(Guid Id) : IRequest<Result<StudioDetailDto>>;

public class GetStudioByIdQueryHandler : IRequestHandler<GetStudioByIdQuery, Result<StudioDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetStudioByIdQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudioDetailDto>> Handle(GetStudioByIdQuery request, CancellationToken cancellationToken)
    {
        var studio = await _context.Studios
            .Where(s => s.Id == request.Id && s.CompanyId == _tenantContext.CompanyId)
            .Select(s => new StudioDetailDto
            {
                Id = s.Id,
                Name = s.Name,
                Details = s.Details,
                ContactInfo = s.ContactInfo,
                Rooms = s.Rooms
                    .Where(r => !r.IsDeleted)
                    .Select(r => new StudioRoomListItemDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Details = r.Details
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (studio == null)
        {
            return Result<StudioDetailDto>.Failure("Studio not found");
        }

        return Result<StudioDetailDto>.Success(studio);
    }
}