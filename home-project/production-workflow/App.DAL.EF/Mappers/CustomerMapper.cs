using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class CustomerMapper : IMapper<CustomerDto, Customer>
{
    public CustomerDto? Map(Customer? entity)
    {
        throw new NotImplementedException();
    }

    public Customer? Map(CustomerDto? entity)
    {
        throw new NotImplementedException();
    }
}