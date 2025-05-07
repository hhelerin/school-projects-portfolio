using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class CustomerRepository(DbContext repositoryDbContext)
    : BaseRepository<CustomerDto, Customer>(repositoryDbContext, new CustomerMapper()), ICustomerRepository;