using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.PlayGame;

public class MoveGrid : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public MoveGrid(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string UserName { get; set; } = default!;
    [BindProperty(SupportsGet = true)]
    public string GameId { get; set; } = default!;

    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;

    // New properties to track button visibility
    public bool CanMoveUp { get; set; }
    public bool CanMoveDown { get; set; }
    public bool CanMoveLeft { get; set; }
    public bool CanMoveRight { get; set; }

    public IActionResult OnGet()
    {
        var dbGame = _gameRepository.LoadGame(GameId);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State)
                    ?? throw new InvalidOperationException();

        TicTacTwoBrain = new TicTacTwoBrain(state.GameConfiguration);
        TicTacTwoBrain.SetGameStateJson(dbGame.State);

        // Determine grid boundaries
        int gridX = TicTacTwoBrain.GetGridLocation()[0];
        int gridY = TicTacTwoBrain.GetGridLocation()[1];
        int gridSize = TicTacTwoBrain.GetGridSize();
        int boardWidth = TicTacTwoBrain.DimX;
        int boardHeight = TicTacTwoBrain.DimY;

        // Check if moving the grid in each direction is possible
        CanMoveUp = gridY > 0; // Moving up is possible if the grid's top edge is not at the board's top edge
        CanMoveDown = gridY + gridSize < boardHeight; // Moving down is possible if the grid's bottom edge doesn't exceed the board's height
        CanMoveLeft = gridX > 0; // Moving left is possible if the grid's left edge is not at the board's left edge
        CanMoveRight = gridX + gridSize < boardWidth; // Moving right is possible if the grid's right edge doesn't exceed the board's width

        return Page();
    }

    public IActionResult OnPost(string direction)
    {
        var dbGame = _gameRepository.LoadGame(GameId);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGame.State)
                    ?? throw new InvalidOperationException();

        TicTacTwoBrain = new TicTacTwoBrain(state.GameConfiguration);
        TicTacTwoBrain.SetGameStateJson(dbGame.State);

        int gridX = TicTacTwoBrain.GetGridLocation()[0];
        int gridY = TicTacTwoBrain.GetGridLocation()[1];

        switch (direction)
        {
            case "up":
                gridY -= 1;
                break;
            case "down":
                gridY += 1;
                break;
            case "left":
                gridX -= 1;
                break;
            case "right":
                gridX += 1;
                break;
        }

        TicTacTwoBrain.MoveGridFindWinner(gridX, gridY);
        
        GameId = _gameRepository.SaveGame(GameId, TicTacTwoBrain.GetGameStateJson(), TicTacTwoBrain.GetGameConfigName());
        return RedirectToPage("/PlayGame/SelectAction", new { GameId, UserName });
    }
}
