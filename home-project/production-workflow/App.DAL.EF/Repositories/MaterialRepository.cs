using App.DAL.Contracts;
using App.DAL.DTO;
using App.DAL.EF.Mappers;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class MaterialRepository(DbContext repositoryDbContext)
    : BaseRepository<MaterialDto, Material>(repositoryDbContext, new MaterialMapper()), IMaterialRepository;