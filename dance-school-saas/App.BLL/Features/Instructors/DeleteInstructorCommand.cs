using App.DAL.EF;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Instructors;

public record DeleteInstructorCommand(Guid Id) : IRequest<Result>;

public class DeleteInstructorCommandHandler : IRequestHandler<DeleteInstructorCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteInstructorCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(DeleteInstructorCommand request, CancellationToken cancellationToken)
    {
        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (instructor == null)
        {
            return Result.Failure("Instructor not found");
        }

        instructor.IsDeleted = true;
        instructor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}