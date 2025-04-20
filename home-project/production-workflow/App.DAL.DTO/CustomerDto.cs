using Contracts;

namespace App.DAL.DTO;

public class CustomerDto : IDomainId
{
    public Guid Id { get; set; }
}