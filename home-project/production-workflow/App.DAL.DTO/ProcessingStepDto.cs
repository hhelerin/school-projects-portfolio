using Contracts;

namespace App.DAL.DTO;

public class ProcessingStepDto : IDomainId
{
    public Guid Id { get; set; }
}