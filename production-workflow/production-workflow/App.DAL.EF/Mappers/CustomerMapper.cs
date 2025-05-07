using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class CustomerMapper : IMapper<CustomerDto, Customer>
{
    public CustomerDto? Map(Customer? entity)
    {
        if (entity == null) return null;
        return new CustomerDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            Address = entity.Address,
            AdditionalInfo = entity.AdditionalInfo,
            Orders = entity.Orders?.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Deadline = o.Deadline
            }).ToList(),
            Shipments = entity.Shipments?.Select(s => new ShipmentDto
            {
                Id = s.Id,
                ShippedOn = s.ShippedOn
            }).ToList(),
            CustomersUsers = entity.CustomersUsers?.Select(cu => new CustomersUsersDto
            {
                Id = cu.Id,
            }).ToList()
        }; 
    }

    public Customer? Map(CustomerDto? dto)
    {
        if (dto == null) return null;

        return new Customer
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            AdditionalInfo = dto.AdditionalInfo,
            Orders = dto.Orders?.Select(o => new Order
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Deadline = o.Deadline
                
            }).ToList(),
            
            Shipments = dto.Shipments?.Select(s => new Shipment
            {
                Id = s.Id,
                ShippedOn = s.ShippedOn
            }).ToList(),
            CustomersUsers = dto.CustomersUsers?.Select(cu => new CustomersUsers
            {
                Id = cu.Id
                //TODO: Decide if it's necessary to map more properties.
            }).ToList()
        };
    }
}