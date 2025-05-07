using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class CustomersUsersMapper : IMapper<CustomersUsersDto, CustomersUsers>
{
    public CustomersUsersDto? Map(CustomersUsers? entity)
    {
        if (entity == null) return null;
        var res = new CustomersUsersDto()
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
        };
        return res;
    }

    public CustomersUsers? Map(CustomersUsersDto? entity)
    {
        if (entity == null) return null;
        var res = new CustomersUsers()
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId
        };
        return res;
    }
    
}