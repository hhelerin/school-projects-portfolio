using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class MaterialMapper : IMapper<App.DAL.DTO.MaterialDto, App.Domain.Material>
{
    public MaterialDto? Map(Material? entity)
    {
        throw new NotImplementedException();
    }

    public Material? Map(MaterialDto? entity)
    {
        throw new NotImplementedException();
    }
}