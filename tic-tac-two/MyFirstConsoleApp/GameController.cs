using DAL;

namespace MyFirstConsoleApp;
using BLL;
using GameBrain;

public static class GameController
{
    private static readonly IGameRepository GameRepository = new GameRepositoryDataBase();
    
    public static string MainLoop(TicTacTwoBrain gameInstance)
{
    while (!gameInstance.IsGameOver())
    {
        //Console.Clear();
        ConsoleUI.Visualizer.DrawBoard(gameInstance);

        string nextMark= gameInstance.GetNextMoveBy() == gameInstance.GetPlayerX() ? "  X  " : "  O  ";
        Console.WriteLine($"It's player {gameInstance.GetNextMoveBy()}'s turn. " +
                          $"Playing as {nextMark}");
        
        bool hasPiecesAtHand = gameInstance.GetNextMoveBy() == gameInstance.GetPlayerX()
            ? gameInstance.GetGamePiecesAtHandByX() > 0
            : gameInstance.GetGamePiecesAtHandByO() > 0;
        
        var canMoveGrid = gameInstance.CanMoveGrid();
        bool restrictionsMet = ((gameInstance.GetRestrictions() >= 0
                                 && (gameInstance.GetRestrictions() <= gameInstance.GetMovesMade()))) 
                               && gameInstance.GetRestrictions() != -1;
        bool hasPiecesOnBoard = gameInstance.HasPiecesOnBoard(gameInstance.GetNextMoveBy());
        
        if (hasPiecesAtHand) Console.WriteLine("P - Place piece");
        if (canMoveGrid && restrictionsMet) Console.WriteLine("M - Move grid");
        if (restrictionsMet && hasPiecesOnBoard) Console.WriteLine("K - Move your piece");
        Console.WriteLine("A - Let AI make that move");
        Console.WriteLine("S - Save game  |  E - Exit");

        var key = Console.ReadKey(true).Key;
        Console.WriteLine();

        bool gameWon = key switch
        {
            ConsoleKey.M when canMoveGrid && restrictionsMet => MoveGrid(gameInstance),
            ConsoleKey.K when restrictionsMet => MovePieceFindWinner(gameInstance),
            ConsoleKey.P when hasPiecesAtHand => PlacePieceFindWinner(gameInstance),
            ConsoleKey.A when true => gameInstance.MakeAMoveByAi(),
            _ => false
        };

        if (gameWon)
        {
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            gameInstance.ResetGame();
            Console.WriteLine("GAME OVER! WE HAVE A WINNER");
            return "";
        }

        switch (key)
        {
            case ConsoleKey.S:
                if (gameInstance.GetSaveGameId() != null)
                {
                    GameRepository.SaveGame(gameInstance.GetSaveGameId(), gameInstance.GetGameStateJson(),
                        gameInstance.GetGameConfigName());
                }
                else
                {
                    string newId = GameRepository.SaveGame(gameInstance.GetGameStateJson(), gameInstance.GetGameConfigName());
                    gameInstance.SetSaveGameId(newId);

                }
                Console.WriteLine("Game is saved.");
                break;
            case ConsoleKey.E:
                Console.Clear();
                return "";
            case ConsoleKey.M:
            case ConsoleKey.K:
            case ConsoleKey.P:
            case ConsoleKey.A:
                break;
            default:
                Console.WriteLine("Invalid key, please try again.");
                break;
        }
    }
    ConsoleUI.Visualizer.DrawBoard(gameInstance);
    Console.WriteLine("GAME OVER: End of story.");
    return "";
}


    private static bool PlacePieceFindWinner(TicTacTwoBrain gameInstance)
    {
        string playersMark= gameInstance.GetNextMoveBy() == gameInstance.GetPlayerX() ? "  X  " : "  O  ";
        Console.Clear();
        var (inputX, inputY) = SelectACell(gameInstance);
        
        while (gameInstance.GetBoard()[inputX][inputY] != "Empty")
        {
            Console.WriteLine($"Please select an empty cell to place your [{playersMark}]");
            (inputX, inputY) = SelectACell(gameInstance);
        }
        return gameInstance.MakeAMoveCheckWinner(inputX, inputY);
    }
    
    
    private static (int inputX, int inputY) SelectACell(TicTacTwoBrain gameInstance)
    {
        var inputX = 0; 
        var inputY = 0;
        var gridWidth = gameInstance.DimX;
        var gridHeight = gameInstance.DimY;
        
        while(true)
        {
            ConsoleUI.Visualizer.DrawBoardWithCursor(gameInstance, inputX, inputY);
            var key = Console.ReadKey(true).Key;
            Console.Clear();
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (inputY > 0) inputY--;
                    break;
                case ConsoleKey.DownArrow:
                    if (inputY < gridHeight - 1) inputY++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (inputX > 0) inputX--;
                    break;
                case ConsoleKey.RightArrow:
                    if (inputX < gridWidth - 1) inputX++;
                    break;
                case ConsoleKey.Enter:
                    return (inputX, inputY);
                default:
                    Console.Clear();
                    Console.WriteLine("Invalid key! Use arrow keys to move and Enter to confirm.");
                    break;
            }
        }
    }

    private static bool MovePieceFindWinner(TicTacTwoBrain gameInstance)
    {
        string nextMark= gameInstance.GetNextMoveBy() == gameInstance.GetPlayerX() ? "  X  " : "  O  ";
        var player = gameInstance.GetNextMoveBy();
        Console.Clear();
        Console.WriteLine($"Select your [{nextMark}] to pick it up");
        var (fromX, fromY) = SelectACell(gameInstance);
        while (gameInstance.GetBoard()[fromX][fromY] != player)
        {
            Console.Clear();
            Console.WriteLine($"Please select a cell with your mark: [{nextMark}]");
            (fromX, fromY) = SelectACell(gameInstance);
        }
        Console.Clear();
        Console.WriteLine($"Select a cell to place your [{nextMark}]");
        var (toX, toY) = SelectACell(gameInstance);
        while (gameInstance.GetBoard()[toX][toY] != "Empty")
        {
            Console.Clear();
            Console.WriteLine($"Please select an empty cell to place your [{nextMark}]");
            (toX, toY) = SelectACell(gameInstance);
        }
        return gameInstance.MovePieceCheckWinner(fromX, fromY, toX, toY);
    }

    private static bool MoveGrid(TicTacTwoBrain gameInstance)
    {
        int currentX = gameInstance.GetGridLocation()[0];
        int currentY = gameInstance.GetGridLocation()[1];

        bool validInput = false;

        var moves = new Dictionary<ConsoleKey, (int dx, int dy)>
        {
            { ConsoleKey.LeftArrow, (-1, 0) },
            { ConsoleKey.RightArrow, (1, 0) },
            { ConsoleKey.UpArrow, (0, -1) },
            { ConsoleKey.DownArrow, (0, 1) }
        };
        do
        {
            Console.WriteLine("Use arrow keys to move the grid (Up, Down, Left, Right):");
            ConsoleKey key = Console.ReadKey(true).Key;
            if (moves.TryGetValue(key, out var move))
            {
                int newX = currentX + move.dx;
                int newY = currentY + move.dy;
                
                if (newX >= 0 && 
                    newX + gameInstance.GetGridSize() <= gameInstance.DimX && 
                    newY >= 0 && 
                    newY + gameInstance.GetGridSize() <= gameInstance.DimY)
                {
                    currentX = newX;
                    currentY = newY;
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("Cannot move there. Grid would go out of bounds.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please use the arrow keys.");
            }

        } while (!validInput);
        return gameInstance.MoveGridFindWinner(currentX, currentY);
    }
}