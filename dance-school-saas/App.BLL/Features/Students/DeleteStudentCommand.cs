using App.DAL.EF;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Students;

public record DeleteStudentCommand(Guid Id) : IRequest<Result>;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteStudentCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (student == null)
        {
            return Result.Failure("Student not found");
        }

        // Check for active packages - this will be fully implemented in Change 7
        // For now, we check if the DbContext has a Packages property
        var hasPackagesProperty = _context.GetType().GetProperty("Packages") != null;
        
        if (hasPackagesProperty)
        {
            // Check for active (non-expired, non-deleted) packages
            // This is a placeholder - will be fully implemented when Package entity is created in Change 7
            // For now, we skip this check as Package entity doesn't exist yet
        }

        student.IsDeleted = true;
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
