using System.ComponentModel.DataAnnotations;
using Domain;

namespace GameBrain;

public class GameConfiguration() : BaseEntity
{
    [MaxLength(128)]
    [Required(ErrorMessage = "Name is required.")]
    [Display(Name = "Configuration name")]
    public string Name { get; set; } = default!;
    
    [Required]
    [Range(3, 13, ErrorMessage = "Board Width must be between 3 and 13.")]
    public int BoardWidth { get; set; } = 5;
    [Required]
    [Range(3, 13, ErrorMessage = "Board Height must be between 3 and 13.")]
    public int BoardHeight { get; set; } = 5;
    [Required]
    [Range(3, int.MaxValue, ErrorMessage = "Grid Size must be at least 3 and fit within the board dimensions.")]
    public int GridSize { get; set; } = 3;
    [Required]
    [Display(Name = "Total moves per game")]
    [Range(20, int.MaxValue, ErrorMessage = "Minimum 20 moves per game. Suggested over 100.")]
    public int MovesPerGame { get; set; } = 3333;
    
    //how many pieces in the row to win
    [Display(Name = "Winning Condition")]
    [Required]
    [Range(3, int.MaxValue, ErrorMessage = "Win Condition must be at least 3.")]
    public int WinCondition { get; set; } = 3;
    
    // -1 disabled
    [Display(Name = "Enable moving grid or gamepiece after N moves")]
    [Range(-1, int.MaxValue, ErrorMessage = "MovePieceOrGridAfterNMoves must be -1 (disabled) or a positive number.")]
    public int MovePieceOrGridAfterNMoves { get; set; } = 4;
    
    [Display(Name = "Pieces per player")]
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Piece Count must be at least equal to Win Condition.")]
    public int PieceCount { get; set; } = 4;
    
    
    public override string ToString() =>  
        $"{Name} \n Board:({BoardWidth}x{BoardHeight})" +
        $"to win: {WinCondition} pieces per row";
    
}