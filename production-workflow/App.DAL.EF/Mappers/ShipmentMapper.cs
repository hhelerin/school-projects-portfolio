using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class ShipmentMapper : IMapper<App.DAL.DTO.ShipmentDto, App.Domain.Shipment>
{
    public ShipmentDto? Map(Shipment? entity)
    {
        throw new NotImplementedException();
    }

    public Shipment? Map(ShipmentDto? entity)
    {
        throw new NotImplementedException();
    }
}