using App.DAL.Contracts;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class CustomersUsersRepository(DbContext repositoryDbContext)
    : BaseRepository<CustomersUsers>(repositoryDbContext), ICustomersUsersRepository{
    public override async Task<IEnumerable<CustomersUsers>> AllAsync(Guid userId = default)
    {
        return await RepositoryDbSet
            .Include(c => c.Customer)
            .Include(c => c.User)
            .ToListAsync();
    }

    public override async Task<CustomersUsers?> FindAsync(Guid id, Guid userId = default)
    {
        return await RepositoryDbSet
            .Include(c => c.Customer)
            .Include(c => c.User)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
};