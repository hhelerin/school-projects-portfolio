using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.DanceStyles;

public record GetDanceStyleByIdQuery(Guid Id) : IRequest<Result<DanceStyleDetailDto>>;

public class GetDanceStyleByIdQueryHandler : IRequestHandler<GetDanceStyleByIdQuery, Result<DanceStyleDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetDanceStyleByIdQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<DanceStyleDetailDto>> Handle(GetDanceStyleByIdQuery request, CancellationToken cancellationToken)
    {
        var style = await _context.DanceStyles
            .Where(ds => ds.Id == request.Id && ds.CompanyId == _tenantContext.CompanyId)
            .Select(ds => new DanceStyleDetailDto
            {
                Id = ds.Id,
                Name = ds.Name,
                Details = ds.Details
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (style == null)
        {
            return Result<DanceStyleDetailDto>.Failure("Dance style not found");
        }

        return Result<DanceStyleDetailDto>.Success(style);
    }
}