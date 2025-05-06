using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class ShipmentRepository(DbContext repositoryDbContext)
    : BaseRepository<ShipmentDto, Shipment>(repositoryDbContext, new ShipmentMapper()), IShipmentRepository
{
    public override async Task<IEnumerable<ShipmentDto>> AllAsync(Guid userId = default)
    {
        return (await RepositoryDbSet
            .Include(s => s.Customer)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<ShipmentDto?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s=> s.Id == id));
    }
};