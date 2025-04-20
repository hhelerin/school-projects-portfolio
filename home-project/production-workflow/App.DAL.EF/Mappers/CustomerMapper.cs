using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class CustomerMapper : IMapper<App.DAL.DTO.CustomerDto, App.Domain.Customer>
{
    public Customer? Map(Customer? domainEntity)
    {
        throw new NotImplementedException();
    }

    public CustomerDto? Map(CustomerDto? dalEntity)
    {
        throw new NotImplementedException();
    }
}