using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OrderMapper : IMapper<App.DAL.DTO.OrderDto, App.Domain.Order>
{
    public Order? Map(Order? domainEntity)
    {
        throw new NotImplementedException();
    }

    public OrderDto? Map(OrderDto? dalEntity)
    {
        throw new NotImplementedException();
    }
}