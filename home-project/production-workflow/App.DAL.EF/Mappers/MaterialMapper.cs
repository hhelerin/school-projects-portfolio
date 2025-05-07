using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.EF.Mappers;

public class MaterialMapper : IMapper<App.DAL.DTO.MaterialDto, App.Domain.Material>
{
    public MaterialDto? Map(Material? entity)
    {
        if (entity == null) return null;
        var res = new MaterialDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Details = entity.Details
        };
        return res;
    }

    public Material? Map(MaterialDto? entity)
    {
        if (entity == null) return null;
        var res = new Material()
        {
            Id = entity.Id,
            Name = entity.Name,
            Details = entity.Details
        };
        return res;
    }
}