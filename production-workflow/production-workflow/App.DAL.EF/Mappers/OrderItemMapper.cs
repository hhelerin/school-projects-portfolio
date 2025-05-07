using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OrderItemMapper : IMapper<OrderItemDto, OrderItem>
{
    public OrderItemDto? Map(OrderItem? entity)
    {
        if (entity == null) return null;
        var res = new OrderItemDto()
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Order = null,
            DetailNumber = entity.DetailNumber,
            LengthMm = entity.LengthMm,
            WidthMm = entity.WidthMm,
            HeightMm = entity.HeightMm,
            Area = entity.Area,
            LinearMeter = entity.LinearMeter,
            Amount = entity.Amount,
            Details = entity.Details
        };
        return res;
    }

    public OrderItem? Map(OrderItemDto? entity)
    {
        if (entity == null) return null;
        var res = new OrderItem()
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            DetailNumber = entity.DetailNumber,
            LengthMm = entity.LengthMm,
            WidthMm = entity.WidthMm,
            HeightMm = entity.HeightMm,
            Area = entity.Area,
            LinearMeter = entity.LinearMeter,
            Amount = entity.Amount,
            Details = entity.Details
        };
        return res;
    }
}