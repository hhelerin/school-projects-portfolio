using Contracts;

namespace App.DAL.DTO;

public class OrderDto: IDomainId
{
    public Guid Id { get; set; }
}