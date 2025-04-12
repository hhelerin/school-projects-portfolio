using App.DAL.Contracts;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class OrderRepository(DbContext repositoryDbContext)
    : BaseRepository<Order>(repositoryDbContext), IOrderRepository
{

    public override async Task<IEnumerable<Order>> AllAsync(Guid userId = default)
    {
        var query = GetIncludes(GetQuery(userId));
        return await query.ToListAsync();
    }
    public override async Task<Order?> FindAsync(Guid id, Guid userId = default)
    {
        var query = GetIncludes(GetQuery(userId));
        return await query.FirstOrDefaultAsync(o => o.Id == id);
    }
    
    private IQueryable<Order> GetIncludes(IQueryable<Order> query)
    {
        return query
            .Include(o => o.Customer)
            .Include(o => o.Material)
            .Include(o => o.Shipment);
    }
};