using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class ShipmentMapper : IMapper<App.DAL.DTO.ShipmentDto, App.Domain.Shipment>
{
    public Shipment? Map(Shipment? domainEntity)
    {
        throw new NotImplementedException();
    }

    public ShipmentDto? Map(ShipmentDto? dalEntity)
    {
        throw new NotImplementedException();
    }
}