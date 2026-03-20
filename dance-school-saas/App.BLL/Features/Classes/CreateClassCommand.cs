using App.DAL.EF;
using App.Domain;
using App.DTO.v1.ClassScheduling;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Classes;

public record CreateClassCommand(CreateClassInputModel Model) : IRequest<Result<Guid>>;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(1000).WithMessage("Details must not exceed 1000 characters");
            
        RuleFor(x => x.Model.StudioRoomId)
            .NotEmpty().WithMessage("Studio room is required");
            
        RuleFor(x => x.Model.InstructorId)
            .NotEmpty().WithMessage("Instructor is required");
            
        RuleFor(x => x.Model.DanceStyleId)
            .NotEmpty().WithMessage("Dance style is required");
            
        RuleFor(x => x.Model.LevelId)
            .NotEmpty().WithMessage("Level is required");
            
        RuleFor(x => x.Model.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");
            
        RuleFor(x => x.Model.RecurrenceEndDate)
            .GreaterThan(x => x.Model.RecurrenceStartDate)
            .WithMessage("End date must be after start date");
    }
}

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, Result<Guid>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateClassCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<Guid>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        // Validate all FK references belong to current tenant
        var studioRoom = await _context.StudioRooms
            .FirstOrDefaultAsync(r => r.Id == request.Model.StudioRoomId && r.Studio.CompanyId == _tenantContext.CompanyId, cancellationToken);
        if (studioRoom == null)
            return Result<Guid>.Failure("Invalid studio room");
            
        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.Id == request.Model.InstructorId && i.CompanyId == _tenantContext.CompanyId, cancellationToken);
        if (instructor == null)
            return Result<Guid>.Failure("Invalid instructor");
            
        var danceStyle = await _context.DanceStyles
            .FirstOrDefaultAsync(d => d.Id == request.Model.DanceStyleId && d.CompanyId == _tenantContext.CompanyId, cancellationToken);
        if (danceStyle == null)
            return Result<Guid>.Failure("Invalid dance style");
            
        var level = await _context.Levels
            .FirstOrDefaultAsync(l => l.Id == request.Model.LevelId && l.CompanyId == _tenantContext.CompanyId, cancellationToken);
        if (level == null)
            return Result<Guid>.Failure("Invalid level");

        // Create the class with recurrence pattern
        var classEntity = new Class
        {
            CompanyId = _tenantContext.CompanyId,
            Name = request.Model.Name,
            Details = request.Model.Details,
            StudioRoomId = request.Model.StudioRoomId,
            InstructorId = request.Model.InstructorId,
            DanceStyleId = request.Model.DanceStyleId,
            LevelId = request.Model.LevelId,
            Capacity = request.Model.Capacity,
            Recurrence = new RecurrencePattern
            {
                DayOfWeek = request.Model.DayOfWeek,
                StartTime = request.Model.StartTime,
                EndTime = request.Model.EndTime,
                RecurrenceStartDate = request.Model.RecurrenceStartDate,
                RecurrenceEndDate = request.Model.RecurrenceEndDate
            }
        };

        // Generate ClassSchedule occurrences
        var schedules = GenerateSchedules(classEntity);
        
        // Check for conflicts
        var conflicts = await CheckConflictsAsync(schedules, cancellationToken);
        if (conflicts.Any())
        {
            return Result<Guid>.Failure($"Scheduling conflict detected with existing class(es): {string.Join(", ", conflicts)}");
        }

        // Save everything in one transaction
        _context.Classes.Add(classEntity);
        _context.ClassSchedules.AddRange(schedules);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(classEntity.Id);
    }

    private List<ClassSchedule> GenerateSchedules(Class classEntity)
    {
        var schedules = new List<ClassSchedule>();
        var currentDate = classEntity.Recurrence.RecurrenceStartDate;
        
        while (currentDate <= classEntity.Recurrence.RecurrenceEndDate)
        {
            if (currentDate.DayOfWeek == classEntity.Recurrence.DayOfWeek)
            {
                schedules.Add(new ClassSchedule
                {
                    CompanyId = classEntity.CompanyId,
                    ClassId = classEntity.Id,
                    Date = currentDate,
                    StartTime = classEntity.Recurrence.StartTime,
                    EndTime = classEntity.Recurrence.EndTime,
                    StudioRoomId = classEntity.StudioRoomId
                });
            }
            currentDate = currentDate.AddDays(1);
        }
        
        return schedules;
    }

    private async Task<List<string>> CheckConflictsAsync(List<ClassSchedule> newSchedules, CancellationToken cancellationToken)
    {
        var conflicts = new List<string>();
        
        foreach (var schedule in newSchedules)
        {
            var conflictingSchedules = await _context.ClassSchedules
                .Where(cs => 
                    cs.StudioRoomId == schedule.StudioRoomId &&
                    cs.Date == schedule.Date &&
                    !cs.IsDeleted &&
                    !cs.IsCancelled &&
                    cs.StartTime < schedule.EndTime &&
                    cs.EndTime > schedule.StartTime)
                .Include(cs => cs.Class)
                .ToListAsync(cancellationToken);
                
            foreach (var conflict in conflictingSchedules)
            {
                conflicts.Add($"{conflict.Date:yyyy-MM-dd} {conflict.StartTime}-{conflict.EndTime} ({conflict.Class.Name})");
            }
        }
        
        return conflicts.Distinct().ToList();
    }
}
