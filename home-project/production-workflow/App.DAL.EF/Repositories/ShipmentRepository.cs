using App.DAL.Contracts;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class ShipmentRepository(DbContext repositoryDbContext)
    : BaseRepository<Shipment>(repositoryDbContext), IShipmentRepository
{
    public override async Task<IEnumerable<Shipment>> AllAsync(Guid userId = default)
    {
        return await RepositoryDbSet
            .Include(s => s.Customer)
            .ToListAsync();
    }

    public override async Task<Shipment?> FindAsync(Guid id, Guid userId = default)
    {
        return await RepositoryDbSet
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s=> s.Id == id);
    }
};