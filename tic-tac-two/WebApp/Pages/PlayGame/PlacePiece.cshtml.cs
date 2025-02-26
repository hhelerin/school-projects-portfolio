using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.PlayGame;

public class PlacePiece : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public PlacePiece(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    [BindProperty(SupportsGet = true)]
    public string UserName { get; set; } = default!;
    [BindProperty(SupportsGet = true)] 
    public string GameId { get; set; } = default!;

    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;

    public IActionResult OnGet(int? x, int? y)
    {
        var dbGame = _gameRepository.LoadGame(GameId);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State)
                    ?? throw new InvalidOperationException();;
        TicTacTwoBrain = new TicTacTwoBrain(state.GameConfiguration);

        TicTacTwoBrain.SetGameStateJson(dbGame.State);

        if (x != null && y != null)
        {
            TicTacTwoBrain.MakeAMoveCheckWinner(x.Value, y.Value);
            
            GameId = _gameRepository.SaveGame(GameId, TicTacTwoBrain.GetGameStateJson(),TicTacTwoBrain.GetGameConfigName());
            if (TicTacTwoBrain.IsGameOver())
            {
                TicTacTwoBrain.SetGameStatus(EGameStatus.GameOver);
                GameId = _gameRepository.SaveGame(GameId, TicTacTwoBrain.GetGameStateJson(),TicTacTwoBrain.GetGameConfigName());
                return RedirectToPage("/PlayGame/SelectAction", new { gameid = GameId, UserName});
            }
            if (TicTacTwoBrain.GetStatus() == EGameStatus.Ongoing)
            {
                return RedirectToPage("/PlayGame/SelectAction", new { gameid = GameId, UserName});
            }
        }
        return Page();
    }
}