using System.ComponentModel.DataAnnotations;
using Domain;

namespace GameBrain;

public class SaveGame : BaseEntity
{
    [MaxLength(128)]
    [Display(Name = "Game")]
    public string Name { get; set; } = default!;
    [MaxLength(10240)]
    public string State { get; set; } = default!;

    public string GetId()
    {
        return Id;
    }
}