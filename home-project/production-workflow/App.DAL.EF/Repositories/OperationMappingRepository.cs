using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class OperationMappingRepository(DbContext repositoryDbContext)
    : BaseRepository<OperationMappingDto, OperationMapping>(repositoryDbContext, new OperationMappingMapper(new OrderMapper(), new ProcessingStepMapper())), IOperationMappingRepository
{
    public override async Task<IEnumerable<OperationMappingDto>> AllAsync(Guid userId = default)
    {
        return (await RepositoryDbSet
            .Include(o => o.Order)
            .Include(o => o.ProcessingStep)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<OperationMappingDto?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(o => o.Order)
            .Include(o => o.ProcessingStep)
            .FirstOrDefaultAsync(o=> o.Id == id));
    }
};