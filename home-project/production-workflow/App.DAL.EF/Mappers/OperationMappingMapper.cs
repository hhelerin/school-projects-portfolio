using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class OperationMappingMapper : IMapper<App.DAL.DTO.OperationMappingDto, App.Domain.OperationMapping>
{
    public OperationMappingDto? Map(OperationMapping? entity)
    {
        throw new NotImplementedException();
    }

    public OperationMapping? Map(OperationMappingDto? entity)
    {
        throw new NotImplementedException();
    }
}