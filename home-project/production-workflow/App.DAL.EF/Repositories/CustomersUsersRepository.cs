using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class CustomersUsersRepository(DbContext repositoryDbContext)
    : BaseRepository<CustomersUsersDto, CustomersUsers>(repositoryDbContext, new CustomersUsersMapper()), ICustomersUsersRepository{
    public override async Task<IEnumerable<CustomersUsersDto>> AllAsync(Guid userId = default)
    {
        return (await RepositoryDbSet
            .Include(c => c.Customer)
            .Include(c => c.User)
            .ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<CustomersUsersDto?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(c => c.Customer)
            .Include(c => c.User)
            .FirstOrDefaultAsync(o => o.Id == id));
    }
};