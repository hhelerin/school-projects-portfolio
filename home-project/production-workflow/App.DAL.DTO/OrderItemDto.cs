using Contracts;

namespace App.DAL.DTO;

public class OrderItemDto : IDomainId
{
    public Guid Id { get; set; }
}