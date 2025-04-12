using App.DAL.Contracts;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class OperationMappingRepository(DbContext repositoryDbContext)
    : BaseRepository<OperationMapping>(repositoryDbContext), IOperationMappingRepository
{
    public override async Task<IEnumerable<OperationMapping>> AllAsync(Guid userId = default)
    {
        return await RepositoryDbSet
            .Include(o => o.Order)
            .Include(o => o.ProcessingStep)
            .ToListAsync();
    }

    public override async Task<OperationMapping?> FindAsync(Guid id, Guid userId = default)
    {
        return await RepositoryDbSet
            .Include(o => o.Order)
            .Include(o => o.ProcessingStep)
            .FirstOrDefaultAsync(o=> o.Id == id);
    }
};