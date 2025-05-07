using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class ProcessingStepMapper : IMapper<App.DAL.DTO.ProcessingStepDto, App.Domain.ProcessingStep>
{
    public ProcessingStepDto? Map(ProcessingStep? entity)
    {
        if (entity == null) return null;
        var res = new ProcessingStepDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Details = entity.Details
        };
        return res;
    }

    public ProcessingStep? Map(ProcessingStepDto? entity)
    {
        if (entity == null) return null;
        
        var res = new ProcessingStep()
        {
            Id = entity.Id,
            Name = entity.Name,
            Details = entity.Details
        };
        return res;
    }
}