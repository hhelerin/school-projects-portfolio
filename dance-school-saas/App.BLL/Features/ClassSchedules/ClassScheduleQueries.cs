using App.DAL.EF;
using App.Domain;
using App.DTO.v1.ClassScheduling;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.ClassSchedules;

public record GetScheduleByWeekQuery(
    DateOnly WeekStartDate,
    Guid? DanceStyleId = null,
    Guid? LevelId = null,
    Guid? StudioRoomId = null,
    Guid? InstructorId = null
) : IRequest<List<ClassScheduleListItemDto>>;

public class GetScheduleByWeekQueryHandler : IRequestHandler<GetScheduleByWeekQuery, List<ClassScheduleListItemDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetScheduleByWeekQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<List<ClassScheduleListItemDto>> Handle(GetScheduleByWeekQuery request, CancellationToken cancellationToken)
    {
        var weekEndDate = request.WeekStartDate.AddDays(6);
        
        var query = _context.ClassSchedules
            .Where(cs => 
                cs.CompanyId == _tenantContext.CompanyId &&
                cs.Date >= request.WeekStartDate &&
                cs.Date <= weekEndDate)
            .Include(cs => cs.Class)
                .ThenInclude(c => c!.DanceStyle)
            .Include(cs => cs.Class)
                .ThenInclude(c => c!.Level)
            .Include(cs => cs.Class)
                .ThenInclude(c => c!.Instructor)
            .Include(cs => cs.StudioRoom)
            .AsQueryable();

        // Apply optional filters
        if (request.DanceStyleId.HasValue)
            query = query.Where(cs => cs.Class!.DanceStyleId == request.DanceStyleId);
            
        if (request.LevelId.HasValue)
            query = query.Where(cs => cs.Class!.LevelId == request.LevelId);
            
        if (request.StudioRoomId.HasValue)
            query = query.Where(cs => cs.StudioRoomId == request.StudioRoomId);
            
        if (request.InstructorId.HasValue)
            query = query.Where(cs => cs.Class!.InstructorId == request.InstructorId);

        var schedules = await query
            .OrderBy(cs => cs.Date)
            .ThenBy(cs => cs.StartTime)
            .ToListAsync(cancellationToken);

        return schedules.Select(cs => new ClassScheduleListItemDto
        {
            Id = cs.Id,
            ClassId = cs.ClassId,
            ClassName = cs.Class!.Name,
            Date = cs.Date,
            StartTime = cs.StartTime,
            EndTime = cs.EndTime,
            StudioRoomName = cs.StudioRoom.Name,
            InstructorName = cs.Class.Instructor.Name,
            DanceStyleName = cs.Class.DanceStyle.Name,
            LevelName = cs.Class.Level.Name,
            IsCancelled = cs.IsCancelled,
            IsException = cs.IsException,
            CancellationReason = cs.CancellationReason
        }).ToList();
    }
}

public record GetClassSchedulesByClassQuery(Guid ClassId) : IRequest<List<ClassScheduleListItemDto>>;

public class GetClassSchedulesByClassQueryHandler : IRequestHandler<GetClassSchedulesByClassQuery, List<ClassScheduleListItemDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetClassSchedulesByClassQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<List<ClassScheduleListItemDto>> Handle(GetClassSchedulesByClassQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var schedules = await _context.ClassSchedules
            .Where(cs => 
                cs.ClassId == request.ClassId && 
                cs.CompanyId == _tenantContext.CompanyId &&
                cs.Date >= today)
            .Include(cs => cs.Class)
                .ThenInclude(c => c!.DanceStyle)
            .Include(cs => cs.Class)
                .ThenInclude(c => c!.Level)
            .Include(cs => cs.Class)
                .ThenInclude(c => c!.Instructor)
            .Include(cs => cs.StudioRoom)
            .OrderBy(cs => cs.Date)
            .ThenBy(cs => cs.StartTime)
            .ToListAsync(cancellationToken);

        return schedules.Select(cs => new ClassScheduleListItemDto
        {
            Id = cs.Id,
            ClassId = cs.ClassId,
            ClassName = cs.Class!.Name,
            Date = cs.Date,
            StartTime = cs.StartTime,
            EndTime = cs.EndTime,
            StudioRoomName = cs.StudioRoom.Name,
            InstructorName = cs.Class.Instructor.Name,
            DanceStyleName = cs.Class.DanceStyle.Name,
            LevelName = cs.Class.Level.Name,
            IsCancelled = cs.IsCancelled,
            IsException = cs.IsException,
            CancellationReason = cs.CancellationReason
        }).ToList();
    }
}
