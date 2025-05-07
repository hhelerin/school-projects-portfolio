using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OperationMappingMapper : IMapper<App.DAL.DTO.OperationMappingDto, App.Domain.OperationMapping>
{
    private readonly OrderMapper _orderMapper;
    private readonly ProcessingStepMapper _processingStepMapper;

    public OperationMappingMapper(OrderMapper orderMapper, ProcessingStepMapper processingStepMapper)
    {
        _orderMapper = orderMapper;
        _processingStepMapper = processingStepMapper;
    }

    public OperationMappingDto? Map(OperationMapping? entity)
    {
        if (entity == null) return null;
        var res = new OperationMappingDto()
        {
            Id = entity.Id,
            ProcessingStepId = entity.ProcessingStepId,
            ProcessingStep = _processingStepMapper.Map(entity.ProcessingStep),
            OrderId = entity.OrderId,
            Order = _orderMapper.Map(entity.Order),
            PrerequisitesObtained = entity.PrerequisitesObtained,
            CompletedAt = entity.CompletedAt,
            Details = entity.Details
        };
        return res;
    }

    public OperationMapping? Map(OperationMappingDto? entity)
    {
        if (entity == null) return null;
        var res = new OperationMapping()
        {
            Id = entity.Id,
            ProcessingStepId = entity.ProcessingStepId,
            ProcessingStep = _processingStepMapper.Map(entity.ProcessingStep),
            OrderId = entity.OrderId,
            Order = _orderMapper.Map(entity.Order),
            PrerequisitesObtained = entity.PrerequisitesObtained,
            CompletedAt = entity.CompletedAt,
            Details = entity.Details
        };
        return res;
    }
}