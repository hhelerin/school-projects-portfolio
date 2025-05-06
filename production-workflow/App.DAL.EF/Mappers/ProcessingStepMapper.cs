using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class ProcessingStepMapper : IMapper<App.DAL.DTO.ProcessingStepDto, App.Domain.ProcessingStep>
{
    public ProcessingStepDto? Map(ProcessingStep? entity)
    {
        throw new NotImplementedException();
    }

    public ProcessingStep? Map(ProcessingStepDto? entity)
    {
        throw new NotImplementedException();
    }
}