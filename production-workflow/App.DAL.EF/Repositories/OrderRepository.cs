using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class OrderRepository(DbContext repositoryDbContext)
    : BaseRepository<OrderDto, Order>(repositoryDbContext, new OrderMapper()), IOrderRepository
{

    public override async Task<IEnumerable<OrderDto>> AllAsync(Guid userId = default)
    {
        var query = GetIncludes(GetQuery(userId));
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }
    public override async Task<OrderDto?> FindAsync(Guid id, Guid userId = default)
    {
        var query = GetIncludes(GetQuery(userId));
        return Mapper.Map(await query.FirstOrDefaultAsync(o => o.Id == id));
    }
    
    private IQueryable<Order> GetIncludes(IQueryable<Order> query)
    {
        return query
            .Include(o => o.Customer)
            .Include(o => o.Material)
            .Include(o => o.Shipment);
    }
};