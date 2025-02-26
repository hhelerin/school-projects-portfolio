using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.PlayGame;

public class MovePiece : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public MovePiece(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string UserName { get; set; } = default!;
    [BindProperty(SupportsGet = true)]
    public string GameId { get; set; } = default!;

    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;

    [TempData]
    public string? SelectedPieceData { get; set; } // Stores the selected piece as "x,y"
    [TempData]
    public string? SelectedTargetData { get; set; } // Stores the selected target as "x,y"

    public (int x, int y)? SelectedPiece =>
        SelectedPieceData != null
            ? ParseCoordinates(SelectedPieceData)
            : null;

    public (int x, int y)? SelectedTarget =>
        SelectedTargetData != null
            ? ParseCoordinates(SelectedTargetData)
            : null;

    public IActionResult OnGet(int x, int y, string type)
    {
        var dbGame = _gameRepository.LoadGame(GameId);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State)
                    ?? throw new InvalidOperationException();

        TicTacTwoBrain = new TicTacTwoBrain(state.GameConfiguration);
        TicTacTwoBrain.SetGameStateJson(dbGame.State);

        if (type == "piece" && TicTacTwoBrain.GetBoard()[x][y] == TicTacTwoBrain.GetNextMoveBy())
        {
            SelectedPieceData = $"{x},{y}";
        }
        else if (type == "target" && TicTacTwoBrain.GetBoard()[x][y] == "Empty")
        {
            SelectedTargetData = $"{x},{y}";

            if (SelectedPiece != null)
            {
                // Attempt to move the piece
                var from = SelectedPiece.Value;
                var to = (x, y);

                TicTacTwoBrain.MovePieceCheckWinner(from.x, from.y, to.x, to.y);
                GameId = _gameRepository.SaveGame(GameId, TicTacTwoBrain.GetGameStateJson(), TicTacTwoBrain.GetGameConfigName());

                ClearSelections();
                return RedirectToPage("/PlayGame/SelectAction", new { GameId, UserName });
            }
        }

        return Page();
    }

    private (int x, int y) ParseCoordinates(string data)
    {
        var parts = data.Split(',').Select(int.Parse).ToArray();
        return (parts[0], parts[1]);
    }

    private void ClearSelections()
    {
        SelectedPieceData = null;
        SelectedTargetData = null;
    }
}
