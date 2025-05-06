using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OrderItemMapper : IMapper<OrderItemDto, OrderItem>
{
    public OrderItemDto? Map(OrderItem? entity)
    {
        throw new NotImplementedException();
    }

    public OrderItem? Map(OrderItemDto? entity)
    {
        throw new NotImplementedException();
    }
}