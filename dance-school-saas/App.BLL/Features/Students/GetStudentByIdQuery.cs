using App.DAL.EF;
using App.DTO.v1.Students;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Students;

public record GetStudentByIdQuery(Guid StudentId) : IRequest<Result<StudentDetailDto>>;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetStudentByIdQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudentDetailDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (student == null)
        {
            return Result<StudentDetailDto>.Failure("Student not found");
        }

        // Get AppUser full name if linked
        string? appUserFullName = null;
        if (student.AppUserId.HasValue)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == student.AppUserId.Value, cancellationToken);
            appUserFullName = appUser != null ? $"{appUser.FirstName} {appUser.LastName}".Trim() : null;
        }

        return Result<StudentDetailDto>.Success(new StudentDetailDto
        {
            Id = student.Id,
            Name = student.Name,
            PersonalId = student.PersonalId,
            ContactInfo = student.ContactInfo,
            Details = student.Details,
            AppUserId = student.AppUserId,
            AppUserFullName = appUserFullName,
            ActivePackages = new List<PackageSummaryDto>(), // Populated in Change 7
            TrialRecords = new List<TrialRecordSummaryDto>(), // Populated in Change 8
            ShowcaseEligibilities = new List<ShowcaseEligibilitySummaryDto>(), // Populated in Change 9
            AttendanceCount = 0, // Will be populated in Change 8
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        });
    }
}
