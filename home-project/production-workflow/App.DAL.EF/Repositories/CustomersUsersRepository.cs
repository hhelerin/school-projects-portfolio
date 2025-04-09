using App.DAL.Contracts;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class CustomersUsersRepository(DbContext repositoryDbContext)
    : BaseRepository<CustomersUsers>(repositoryDbContext), ICustomersUsersRepository;