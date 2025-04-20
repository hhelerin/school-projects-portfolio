using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class MaterialMapper : IMapper<App.DAL.DTO.MaterialDto, App.Domain.Material>
{
    public Material? Map(Material? domainEntity)
    {
        throw new NotImplementedException();
    }

    public MaterialDto? Map(MaterialDto? dalEntity)
    {
        throw new NotImplementedException();
    }
}