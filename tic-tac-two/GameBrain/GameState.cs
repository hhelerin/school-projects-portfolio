namespace GameBrain;

public class GameState
{
    public string? SaveGameId { get; set; }
    public string[][] GameBoard { get; set; }
    public string NextMoveBy { get; set; } = "X";
    public EGameStatus GameStatus { get; set; } = EGameStatus.Ongoing;
    public EGameType GameType { get; set; } = EGameType.PlayerVersusPlayer;
    public int GamePiecesAtHandByX { get; set; }
    public int GamePiecesAtHandByO { get; set; }
    public string PlayerX { get; set; } = "X";
    public string PlayerO { get; set; } = "O";
    public readonly string EmptyCell = "Empty";
    public int[] GridLocation { get; set; } = new int[2];
    public int GridSize { get; set; }
    public int MovesPerGame { get; set; }
    public int MovePieceOrGridAfterNMoves { get; set; }
    public int WinCondition { get; set; }
    public int MovesMadeDuringGame { get; set; } = 0;
    public GameConfiguration GameConfiguration { get; set; }

    public GameState(string[][] gameBoard, GameConfiguration gameConfiguration)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;
        MovesPerGame = gameConfiguration.MovesPerGame;
        if (gameConfiguration.PieceCount < 2)
        {
            throw new ArgumentException("Minimum two game pieces per player");
        }
        GamePiecesAtHandByO = gameConfiguration.PieceCount;
        GamePiecesAtHandByX = gameConfiguration.PieceCount;
        if (gameConfiguration.WinCondition > gameConfiguration.GridSize)
        {
            throw new ArgumentException("Winning row must fit in the grid");
        }
        WinCondition = gameConfiguration.WinCondition;
        if (gameConfiguration.GridSize > gameConfiguration.BoardWidth ||
            gameConfiguration.GridSize > gameConfiguration.BoardHeight)
        {
            throw new ArgumentException("Grid must fit in the game board");
        }
        GridSize = gameConfiguration.GridSize;
        
        GridLocation[0] = (gameConfiguration.BoardWidth - gameConfiguration.GridSize)/2;
        GridLocation[1] = (gameConfiguration.BoardHeight - gameConfiguration.GridSize)/2;
        
        MovePieceOrGridAfterNMoves = gameConfiguration.MovePieceOrGridAfterNMoves;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
