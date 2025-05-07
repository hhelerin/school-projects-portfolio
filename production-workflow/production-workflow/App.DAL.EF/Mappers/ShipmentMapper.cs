using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class ShipmentMapper : IMapper<App.DAL.DTO.ShipmentDto, App.Domain.Shipment>
{
    public ShipmentDto? Map(Shipment? entity)
    {
        if (entity == null) return null;
        var res = new ShipmentDto()
        {
        Id = entity.Id,
        ShippedOn = entity.ShippedOn,
        Method =  entity.Method,
        Details = entity.Details,
        CustomerId = entity.CustomerId,
        Customer = entity.Customer == null ? null : new CustomerDto()
        {
            Id = entity.Customer.Id,
            Name = entity.Customer.Name,
        }
        };
        return res;
    }

    public Shipment? Map(ShipmentDto? entity)
    {
        if (entity == null) return null;
        var res = new Shipment()
        {
            Id = entity.Id,
            ShippedOn = entity.ShippedOn,
            Method =  entity.Method,
            Details = entity.Details,
            CustomerId = entity.CustomerId
        };
        return res;
    }
}