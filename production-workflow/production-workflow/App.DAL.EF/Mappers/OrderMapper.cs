using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OrderMapper : IMapper<App.DAL.DTO.OrderDto, App.Domain.Order>
{
    public OrderDto? Map(Order? entity)
    {
        if (entity == null) return null;
        return new OrderDto()
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Customer = entity.Customer == null ? null : new CustomerDto()
            {
                Id = entity.Customer.Id,
                Name = entity.Customer.Name,
            },
            MaterialId = entity.MaterialId,
            Material = entity.Material == null ? null : new MaterialDto()
            {
                Id = entity.Material.Id,
                Name = entity.Material.Name
            },
            OrderNumber = entity.OrderNumber,
            Name = entity.Name,
            OrderDate = entity.OrderDate,
            Deadline = entity.Deadline,
            TotalAmount = entity.TotalAmount,
            TotalArea = entity.TotalArea,
            LinearMeter = entity.LinearMeter,
            Details = entity.Details,
            Status = entity.Status,
            ShipmentID = entity.ShipmentID,
            PalletNumber = entity.PalletNumber,
            BillingDate = entity.BillingDate,
        };
    }

    public Order? Map(OrderDto? dto)
    {
        if (dto == null) return null;

        return new Order
        {
            Id = dto.Id,
            CustomerId = dto.CustomerId,
            MaterialId = dto.MaterialId,
            OrderNumber = dto.OrderNumber,
            Name = dto.Name,
            OrderDate = dto.OrderDate,
            Deadline = dto.Deadline,
            TotalAmount = dto.TotalAmount,
            TotalArea = dto.TotalArea,
            LinearMeter = dto.LinearMeter,
            Details = dto.Details,
            Status = dto.Status,
            ShipmentID = dto.ShipmentID,
            PalletNumber = dto.PalletNumber,
            BillingDate = dto.BillingDate
            
        };
    }
}