using App.DAL.EF;
using App.Domain;
using App.DTO.v1.ClassScheduling;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Classes;

public record GetClassByIdQuery(Guid ClassId) : IRequest<ClassDetailDto?>;

public class GetClassByIdQueryHandler : IRequestHandler<GetClassByIdQuery, ClassDetailDto?>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetClassByIdQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<ClassDetailDto?> Handle(GetClassByIdQuery request, CancellationToken cancellationToken)
    {
        var classEntity = await _context.Classes
            .Where(c => c.Id == request.ClassId && c.CompanyId == _tenantContext.CompanyId)
            .Include(c => c.StudioRoom)
            .Include(c => c.Instructor)
            .Include(c => c.DanceStyle)
            .Include(c => c.Level)
            .FirstOrDefaultAsync(cancellationToken);
            
        if (classEntity == null)
            return null;

        var today = DateOnly.FromDateTime(DateTime.Today);
        var upcomingCount = await _context.ClassSchedules
            .CountAsync(cs => cs.ClassId == classEntity.Id && cs.Date >= today && !cs.IsDeleted, cancellationToken);

        return new ClassDetailDto
        {
            Id = classEntity.Id,
            Name = classEntity.Name,
            Details = classEntity.Details,
            DanceStyleName = classEntity.DanceStyle.Name,
            LevelName = classEntity.Level.Name,
            InstructorName = classEntity.Instructor.Name,
            StudioRoomName = classEntity.StudioRoom.Name,
            Capacity = classEntity.Capacity,
            RecurrenceSummary = $"{classEntity.Recurrence.DayOfWeek}s {classEntity.Recurrence.StartTime:HH:mm}–{classEntity.Recurrence.EndTime:HH:mm}",
            DayOfWeek = classEntity.Recurrence.DayOfWeek,
            StartTime = classEntity.Recurrence.StartTime,
            EndTime = classEntity.Recurrence.EndTime,
            RecurrenceStartDate = classEntity.Recurrence.RecurrenceStartDate,
            RecurrenceEndDate = classEntity.Recurrence.RecurrenceEndDate,
            UpcomingScheduleCount = upcomingCount
        };
    }
}

public record GetClassListQuery : IRequest<List<ClassListItemDto>>;

public class GetClassListQueryHandler : IRequestHandler<GetClassListQuery, List<ClassListItemDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetClassListQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<List<ClassListItemDto>> Handle(GetClassListQuery request, CancellationToken cancellationToken)
    {
        var classes = await _context.Classes
            .Where(c => c.CompanyId == _tenantContext.CompanyId)
            .Include(c => c.StudioRoom)
            .Include(c => c.Instructor)
            .Include(c => c.DanceStyle)
            .Include(c => c.Level)
            .ToListAsync(cancellationToken);

        return classes.Select(c => new ClassListItemDto
        {
            Id = c.Id,
            Name = c.Name,
            DanceStyleName = c.DanceStyle.Name,
            LevelName = c.Level.Name,
            InstructorName = c.Instructor.Name,
            StudioRoomName = c.StudioRoom.Name,
            Capacity = c.Capacity,
            RecurrenceSummary = $"{c.Recurrence.DayOfWeek}s {c.Recurrence.StartTime:HH:mm}–{c.Recurrence.EndTime:HH:mm}"
        }).ToList();
    }
}
