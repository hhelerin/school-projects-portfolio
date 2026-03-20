using App.DAL.EF;
using App.Domain;
using App.DTO.v1.ClassScheduling;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.ClassSchedules;

public record EditClassScheduleInstanceCommand(
    Guid ClassScheduleId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    Guid StudioRoomId,
    string? Details
) : IRequest<Result>;

public class EditClassScheduleInstanceCommandValidator : AbstractValidator<EditClassScheduleInstanceCommand>
{
    public EditClassScheduleInstanceCommandValidator()
    {
        RuleFor(x => x.ClassScheduleId)
            .NotEmpty().WithMessage("Class schedule ID is required");
            
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");
            
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required");
            
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time");
            
        RuleFor(x => x.StudioRoomId)
            .NotEmpty().WithMessage("Studio room is required");
    }
}

public class EditClassScheduleInstanceCommandHandler : IRequestHandler<EditClassScheduleInstanceCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public EditClassScheduleInstanceCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(EditClassScheduleInstanceCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _context.ClassSchedules
            .FirstOrDefaultAsync(cs => cs.Id == request.ClassScheduleId && cs.CompanyId == _tenantContext.CompanyId, cancellationToken);
            
        if (schedule == null)
            return Result.Failure("Schedule not found");

        // Check for conflicts
        var conflicts = await _context.ClassSchedules
            .Where(cs => 
                cs.Id != request.ClassScheduleId &&
                cs.StudioRoomId == request.StudioRoomId &&
                cs.Date == request.Date &&
                !cs.IsDeleted &&
                !cs.IsCancelled &&
                cs.StartTime < request.EndTime &&
                cs.EndTime > request.StartTime)
            .Include(cs => cs.Class)
            .ToListAsync(cancellationToken);
            
        if (conflicts.Any())
        {
            return Result.Failure("Scheduling conflict detected");
        }

        // Update fields and set IsException = true
        schedule.Date = request.Date;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
        schedule.StudioRoomId = request.StudioRoomId;
        schedule.Details = request.Details;
        schedule.IsException = true;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}

public record CancelClassScheduleInstanceCommand(
    Guid ClassScheduleId,
    string CancellationReason
) : IRequest<Result>;

public class CancelClassScheduleInstanceCommandValidator : AbstractValidator<CancelClassScheduleInstanceCommand>
{
    public CancelClassScheduleInstanceCommandValidator()
    {
        RuleFor(x => x.ClassScheduleId)
            .NotEmpty().WithMessage("Class schedule ID is required");
            
        RuleFor(x => x.CancellationReason)
            .NotEmpty().WithMessage("Cancellation reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}

public class CancelClassScheduleInstanceCommandHandler : IRequestHandler<CancelClassScheduleInstanceCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CancelClassScheduleInstanceCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(CancelClassScheduleInstanceCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _context.ClassSchedules
            .FirstOrDefaultAsync(cs => cs.Id == request.ClassScheduleId && cs.CompanyId == _tenantContext.CompanyId, cancellationToken);
            
        if (schedule == null)
            return Result.Failure("Schedule not found");

        schedule.IsCancelled = true;
        schedule.CancellationReason = request.CancellationReason;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
