using App.DAL.Contracts;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class OrderItemRepository(DbContext repositoryDbContext)
    : BaseRepository<OrderItem>(repositoryDbContext), IOrderItemRepository
{
    public override async Task<IEnumerable<OrderItem>> AllAsync(Guid userId = default)
    {
        return await RepositoryDbSet
            .Include(o => o.Order)
            .ToListAsync();
    }
};