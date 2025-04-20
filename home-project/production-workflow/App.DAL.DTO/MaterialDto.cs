using Contracts;

namespace App.DAL.DTO;

public class MaterialDto : IDomainId
{
    public Guid Id { get; set; }
}