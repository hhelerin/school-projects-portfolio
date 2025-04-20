using Contracts;

namespace App.DAL.DTO;

public class ShipmentDto : IDomainId
{
    public Guid Id { get; set; }
}