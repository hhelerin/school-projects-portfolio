using Contracts;

namespace App.DAL.DTO;

public class OperationMappingDto : IDomainId
{
    public Guid Id { get; set; }
}