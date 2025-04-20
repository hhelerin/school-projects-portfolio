using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class ProcessingStepMapper : IMapper<App.DAL.DTO.ProcessingStepDto, App.Domain.ProcessingStep>
{
    public ProcessingStep? Map(ProcessingStep? domainEntity)
    {
        throw new NotImplementedException();
    }

    public ProcessingStepDto? Map(ProcessingStepDto? dalEntity)
    {
        throw new NotImplementedException();
    }
}