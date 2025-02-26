using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.PlayGame;

public class SelectAction : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public SelectAction(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    [BindProperty(SupportsGet = true)]
    public string UserName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public string? User2Name { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string GameId { get; set; } = default!;

    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    public bool CanPlacePiece { get; set; }
    public bool CanMoveGrid { get; set; }
    public bool CanMovePiece { get; set; }

    public IActionResult OnGet()
    {
        var dbGame = _gameRepository.LoadGame(GameId);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State)
                    ?? throw new InvalidOperationException();

        TicTacTwoBrain = new TicTacTwoBrain(state);
        
        if (User2Name != null)
        {
            TicTacTwoBrain.SetPlayerO(User2Name);
            TicTacTwoBrain.SetNextMoveBy(User2Name);
            GameId = _gameRepository.SaveGame(GameId, TicTacTwoBrain.GetGameStateJson(),TicTacTwoBrain.GetGameConfigName() );
            User2Name = null;
        }
        
        CanPlacePiece = (TicTacTwoBrain.GetPlayerX() == UserName && (TicTacTwoBrain.GetGamePiecesAtHandByX() > 0)) || (TicTacTwoBrain.GetPlayerO() == UserName && TicTacTwoBrain.GetGamePiecesAtHandByO() > 0);
        CanMoveGrid = TicTacTwoBrain.CanMoveGrid() && (TicTacTwoBrain.GetRestrictions() == -1 || 
                                                       TicTacTwoBrain.GetMovesMade() >= TicTacTwoBrain.GetRestrictions());
        CanMovePiece = TicTacTwoBrain.HasPiecesOnBoard(TicTacTwoBrain.GetNextMoveBy()) && 
                       (TicTacTwoBrain.GetRestrictions() == -1 || 
                        TicTacTwoBrain.GetMovesMade() >= TicTacTwoBrain.GetRestrictions());

        return Page();
    }
    public IActionResult OnPostAIMove()
    {
        var dbGame = _gameRepository.LoadGame(GameId);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State)
                    ?? throw new InvalidOperationException();
        
        TicTacTwoBrain = new TicTacTwoBrain(state.GameConfiguration);
        TicTacTwoBrain.SetGameStateJson(dbGame.State);

        TicTacTwoBrain.MakeAMoveByAi();

        GameId = _gameRepository.SaveGame(
            GameId,
            TicTacTwoBrain.GetGameStateJson(),
            TicTacTwoBrain.GetGameConfigName()
        );
        
        if (TicTacTwoBrain.IsGameOver())
        {
            TicTacTwoBrain.SetGameStatus(EGameStatus.GameOver);
            GameId = _gameRepository.SaveGame(GameId, TicTacTwoBrain.GetGameStateJson(),TicTacTwoBrain.GetGameConfigName());
        }

        return RedirectToPage("/PlayGame/SelectAction",new { gameid = GameId, UserName });
    }
}
