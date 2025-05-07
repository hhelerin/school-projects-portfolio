using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class OrderItemRepository(DbContext repositoryDbContext)
    : BaseRepository<OrderItemDto, OrderItem>(repositoryDbContext, new OrderItemMapper()), IOrderItemRepository
{
    public override async Task<IEnumerable<OrderItemDto>> AllAsync(Guid userId = default)
    {
        return (await RepositoryDbSet
            .Include(o => o.Order)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }
};