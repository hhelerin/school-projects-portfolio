using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OrderMapper : IMapper<App.DAL.DTO.OrderDto, App.Domain.Order>
{
    public OrderDto? Map(Order? entity)
    {
        throw new NotImplementedException();
    }

    public Order? Map(OrderDto? entity)
    {
        throw new NotImplementedException();
    }
}