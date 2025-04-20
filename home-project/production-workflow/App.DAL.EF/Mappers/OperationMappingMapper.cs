using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OperationMappingMapper : IMapper<App.DAL.DTO.OperationMappingDto, App.Domain.OperationMapping>
{
    public OperationMapping? Map(OperationMapping? domainEntity)
    {
        throw new NotImplementedException();
    }

    public OperationMappingDto? Map(OperationMappingDto? dalEntity)
    {
        throw new NotImplementedException();
    }
}