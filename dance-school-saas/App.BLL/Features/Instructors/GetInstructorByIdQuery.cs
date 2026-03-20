using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Instructors;

public record GetInstructorByIdQuery(Guid Id) : IRequest<Result<InstructorDetailDto>>;

public class GetInstructorByIdQueryHandler : IRequestHandler<GetInstructorByIdQuery, Result<InstructorDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetInstructorByIdQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<InstructorDetailDto>> Handle(GetInstructorByIdQuery request, CancellationToken cancellationToken)
    {
        var instructor = await _context.Instructors
            .Where(i => i.Id == request.Id && i.CompanyId == _tenantContext.CompanyId)
            .Select(i => new InstructorDetailDto
            {
                Id = i.Id,
                Name = i.Name,
                PersonalId = i.PersonalId,
                ContactInfo = i.ContactInfo,
                AppUserId = i.AppUserId,
                AppUserEmail = i.AppUserId != null ? _context.Users.Where(u => u.Id == i.AppUserId).Select(u => u.Email).FirstOrDefault() : null,
                Details = i.Details
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (instructor == null)
        {
            return Result<InstructorDetailDto>.Failure("Instructor not found");
        }

        return Result<InstructorDetailDto>.Success(instructor);
    }
}