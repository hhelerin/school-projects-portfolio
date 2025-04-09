using App.DAL.Contracts;
using App.Domain;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class ShipmentRepository(DbContext repositoryDbContext)
    : BaseRepository<Shipment>(repositoryDbContext), IShipmentRepository;