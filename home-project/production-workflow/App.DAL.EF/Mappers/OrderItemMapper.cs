using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OrderItemMapper : IMapper<App.DAL.DTO.OrderItemDto, App.Domain.Order >
{
    public Order? Map(Order? domainEntity)
    {
        throw new NotImplementedException();
    }

    public OrderItemDto? Map(OrderItemDto? dalEntity)
    {
        throw new NotImplementedException();
    }
}