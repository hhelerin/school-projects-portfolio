namespace GameBrain;


public class TicTacTwoBrain
{
    private GameState _gameState;
    
    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        var gameBoard = new string[gameConfiguration.BoardWidth][];
    
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new string[gameConfiguration.BoardHeight];
        
            for (var y = 0; y < gameBoard[x].Length; y++)
            {
                gameBoard[x][y] = "Empty";
            }
        }
    
        _gameState = new GameState(gameBoard, gameConfiguration);
    }

    public TicTacTwoBrain(GameState state)
    {
        _gameState = state;
    }
    
    public int DimX => _gameState.GameBoard.Length;
    public int DimY => _gameState.GameBoard[0].Length;
    
    public void SetSaveGameId(string newGameId)
    {
        _gameState.SaveGameId = newGameId;
    }
    public string? GetSaveGameId()
    {
        return _gameState.SaveGameId;
    }
    public string GetGameStateJson()
    {
        return _gameState.ToString();
    }
    
    public void SetGameStateJson(string dbGameGameState)
    {
        _gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGameGameState);
    }

    public string GetGameConfigName()
    {
        return _gameState.GameConfiguration.Name;
    }
    
    public EGameType GetGameType()
    {
        return _gameState.GameType;
    }

    public void SetGameType(EGameType gameType)
    {
        _gameState.GameType = gameType;
    }

    public void SetNextMoveBy(string player)
    {
        _gameState.NextMoveBy = player;
    }

    public void SetPlayerX(string playerName)
    {
        _gameState.PlayerX = playerName;
    }
    
    public void SetPlayerO(string playerName)
    {
        _gameState.PlayerO = playerName;
    }

    public string GetPlayerX()
    {
        return _gameState.PlayerX;
    }
    public string GetPlayerO()
    {
        return _gameState.PlayerO;
    }
    public string GetNextMoveBy()
    {
        return _gameState.NextMoveBy;
    }

    public int GetRestrictions()
    {
        return _gameState.MovePieceOrGridAfterNMoves;
    }

    public EGameStatus GetStatus()
    {
        return _gameState.GameStatus;
    }
    public void SetGameStatus(EGameStatus gameStatus)
    {
        _gameState.GameStatus = gameStatus;
    }

    public int GetMovesMade()
    {
        return _gameState.MovesMadeDuringGame;
    }

    public int GetWinCondition()
    {
        return _gameState.WinCondition;
    }

    public int GetMovesLeft()
    {
        return _gameState.MovesPerGame - _gameState.MovesMadeDuringGame;
    }
    
    public string[][] GetBoard()
    {
        var copyOfBoard = new string[_gameState.GameBoard.GetLength(0)][];
        
        for (var x = 0; x < _gameState.GameBoard.Length; x++)
        {
            copyOfBoard[x] = new string[_gameState.GameBoard[x].Length];
            for (var y = 0; y < _gameState.GameBoard[x].Length; y++)
            {
                copyOfBoard[x][y] = _gameState.GameBoard[x][y];
            }
        }
        return copyOfBoard;
    }

    public int[] GetGridLocation()
    {
        return _gameState.GridLocation;
    }

    public int GetGridSize()
    {
        return _gameState.GridSize;
    }

    public int GetGamePiecesAtHandByX()
    {
        return _gameState.GamePiecesAtHandByX;
    }
    
    public int GetGamePiecesAtHandByO()
    {
        return _gameState.GamePiecesAtHandByO;
    }

    public bool MakeAMoveCheckWinner(int x, int y)
    {
        string player = _gameState.NextMoveBy;
        
        if(player == GetPlayerX() && _gameState.GamePiecesAtHandByX > 0){
            _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
            _gameState.GamePiecesAtHandByX--;
        }else if (player == GetPlayerO() && _gameState.GamePiecesAtHandByO > 0)
        {
            _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
            _gameState.GamePiecesAtHandByO--;
        }
        
        SwitchPlayerIncreaseMovesMade();
        return IsWinner(player);
    }
    
    public bool MovePieceCheckWinner(int fromX, int fromY, int toX, int toY)
    {
        string player = GetNextMoveBy();

        _gameState.GameBoard[fromX][fromY] = _gameState.EmptyCell;
        _gameState.GameBoard[toX][toY] = player;
        
        SwitchPlayerIncreaseMovesMade();
        return IsWinner(player);
    }
    
    private void SwitchPlayerIncreaseMovesMade()
    {
        _gameState.NextMoveBy = _gameState.NextMoveBy == GetPlayerX() ? GetPlayerO() : GetPlayerX();
        _gameState.MovesMadeDuringGame++;
    }

    private bool IsWinner(string player)
    {
        int startX = _gameState.GridLocation[0];
        int startY = _gameState.GridLocation[1];
        int endX = startX + _gameState.GridSize;
        int endY = startY + _gameState.GridSize;
        
        bool CheckDirection(int startRow, int startCol, int dirRow, int dirCol)
        {
            for (int i = 0; i < _gameState.WinCondition; i++)
            {
                int row = startRow + i * dirRow;
                int col = startCol + i * dirCol;
                if (row < startX 
                    || row >= endX 
                    || col < startY 
                    || col >= endY 
                    || _gameState.GameBoard[row][col] != player)
                {
                    return false;
                }
            }
            _gameState.GameStatus = player == GetPlayerX() ? EGameStatus.XWon : EGameStatus.OWon;
            return true;
        }
        for (int row = startX; row < endX; row++)
        {
            for (int col = startY; col < endY; col++)
            {
                if (CheckDirection(row, col, 0, 1) ||        // Horizontal
                    CheckDirection(row, col, 1, 0) ||        // Vertical
                    CheckDirection(row, col, 1, 1) ||        // \ Diagonal
                    CheckDirection(row, col, -1, 1))         // / Diagonal
                {
                    _gameState.GameStatus = player == GetPlayerX() ? EGameStatus.XWon : EGameStatus.OWon;
                    return true;
                }
            }
        }
        return false;
    }

    public void ResetGame()
    {
        var gameBoard = new string[_gameState.GameConfiguration.BoardWidth][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new string[_gameState.GameConfiguration.BoardHeight];
        }
        _gameState.GameBoard = gameBoard;
        _gameState.NextMoveBy = GetPlayerX();
        _gameState.GameStatus = EGameStatus.Ongoing;
    }
    

    public bool IsGameOver()
    {
        if (_gameState.MovesMadeDuringGame == _gameState.MovesPerGame)
        {
            Console.WriteLine("No more moves left.");
            _gameState.GameStatus = EGameStatus.GameOver;
            return true;
        }

        for (int i = 0; i < DimX; i++)
        {
            for (int j = 0; j < DimY; j++)
            {
                if (_gameState.GameBoard[i] [j] == _gameState.EmptyCell)
                {
                    return false;
                }
            }
        }
        Console.WriteLine("No more empty spaces left");
        _gameState.GameStatus = EGameStatus.GameOver;
        return true;
    }
    
    public bool CanMoveGrid()
    {
        bool canMoveGrid = GetGridSize() < DimX ||
                           GetGridSize() < DimY;
        return canMoveGrid;
    }

    public bool MoveGridFindWinner(int newX, int newY)
    {
        string player = GetNextMoveBy();
        _gameState.GridLocation[0] = newX;
        _gameState.GridLocation[1] = newY;
        SwitchPlayerIncreaseMovesMade();
        return IsWinner(player) || IsWinner(GetNextMoveBy());
    }
    
    public bool HasPiecesOnBoard(string nextPlayer)
    {
        for (int i = 0; i < DimX; i++)
        {
            for (int j = 0; j < DimY; j++)
            {
                if (_gameState.GameBoard[i] [j] == nextPlayer)
                {
                    return true;
                }
            }
        }return false;
    }
    
    
    public bool MakeAMoveByAi()
{
    var aiPlayer = _gameState.NextMoveBy;
    var opponent = aiPlayer == GetPlayerX() ? GetPlayerO() : GetPlayerX(); 
    var aiPiecesAtHand = aiPlayer == GetPlayerX() ? GetGamePiecesAtHandByX() : GetGamePiecesAtHandByO();

    if (aiPiecesAtHand > 0)
    {
        // Try placing the piece in an empty cell and check if it results in a win
        if (TryWinningMove(aiPlayer))
        {
            return true; // AI made a winning move
        }
        if (TryBlockOpponentMove(opponent))
        {
            return false; // Blocked opponent's winning move, but didn't win
        }
        
        if (_gameState.MovePieceOrGridAfterNMoves >= _gameState.MovesMadeDuringGame)
        {
            // Try moving a piece
            if (TryWinByMovePiece(aiPlayer))
            {
                return true; // AI moved piece and won
            }
                    
            // Try moving the grid
            if (CanMoveGrid() && TryWinByMoveGrid())
            {
                return true; // AI moved grid and won
            }
        }
        
        // If no winning or blocking move, place a piece in the center if possible
        PlacePieceClosestToCenter(aiPlayer);
        return false; // AI made a move, but no win
    }
    // Try moving a piece
    if (TryWinByMovePiece(aiPlayer))
    {
        return true; // AI moved piece and won
    }
                    
    // Try moving the grid
    if (CanMoveGrid() && TryWinByMoveGrid())
    {
        return true; // AI moved grid and won
    }
    
    MoveAiPieceRandomly(aiPlayer);
    return false; // AI made a move, but no win
    
}


// Tries to find and make a winning move for the AI player
private bool TryWinningMove(string aiPlayer)
{
    for (int x = 0; x < DimX; x++)
    {
        for (int y = 0; y < DimY; y++)
        {
            if (_gameState.GameBoard[x][y] == _gameState.EmptyCell)
            {
                _gameState.GameBoard[x][y] = aiPlayer;
                if (IsWinner(aiPlayer))
                {
                    SwitchPlayerIncreaseMovesMade();
                    return true; // AI made a winning move
                }
                _gameState.GameBoard[x][y] = _gameState.EmptyCell; // Undo move
            }
        }
    }
    return false;
}

// Attempts to block the opponent's winning move
private bool TryBlockOpponentMove(string opponent)
{
    for (int x = 0; x < DimX; x++)
    {
        for (int y = 0; y < DimY; y++)
        {
            if (_gameState.GameBoard[x][y] == _gameState.EmptyCell)
            {
                _gameState.GameBoard[x][y] = opponent;
                if (IsWinner(opponent))
                {
                    _gameState.GameBoard[x][y] = _gameState.NextMoveBy; // Block opponent's win
                    _gameState.GameStatus = EGameStatus.Ongoing;
                    if (GetPlayerX() == opponent)
                    {
                        _gameState.GamePiecesAtHandByO--;
                    }else
                        _gameState.GamePiecesAtHandByX--;
                    SwitchPlayerIncreaseMovesMade();
                    return true; // Blocked opponent's winning move
                }
                _gameState.GameBoard[x][y] = _gameState.EmptyCell; // Undo move
                
            }
        }
    }
    return false;
}

private bool PlacePieceClosestToCenter(string aiPlayer)
{
    // Center coordinates of the board
    int centerX = DimX / 2;
    int centerY = DimY / 2;

    // Initialize variables to track the closest empty cell
    int? closestX = null;
    int? closestY = null;
    int minDistance = int.MaxValue;

    // Iterate over all cells to find the closest empty cell
    for (int x = 0; x < DimX; x++)
    {
        for (int y = 0; y < DimY; y++)
        {
            if (_gameState.GameBoard[x][y] == _gameState.EmptyCell)
            {
                // Calculate Manhattan distance to the center
                int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);

                // Update the closest cell if this cell is closer
                if (distance < minDistance)
                {
                    closestX = x;
                    closestY = y;
                    minDistance = distance;
                }
            }
        }
    }

    // If a valid empty cell was found, place the piece
    if (closestX.HasValue && closestY.HasValue)
    {
        _gameState.GameBoard[closestX.Value][closestY.Value] = aiPlayer;
        if (GetPlayerX() == aiPlayer)
        {
            _gameState.GamePiecesAtHandByX--;
        }else
            _gameState.GamePiecesAtHandByO--;
        
        SwitchPlayerIncreaseMovesMade();
        return true;
    }
    // No empty cell found
    return false;
}


// Checks if the AI can move the grid and tries to make a winning move by doing so
private bool TryWinByMoveGrid()
{
    // Get the current grid location and grid size
    var currentGridLocation = GetGridLocation();
    int currentX = currentGridLocation[0];
    int currentY = currentGridLocation[1];
    int gridSize = GetGridSize();

    // Try moving the grid in four directions: up, down, left, right
    // Direction: Up
    if (currentY > 0) // Can move up?
    {
        // Move the grid up
        _gameState.GridLocation = new int[] { currentX, currentY - 1 };
        if (IsWinner(GetNextMoveBy()))
        {
            return true; // AI moved grid and won
        }
        // Undo the move
        _gameState.GridLocation = [currentX, currentY];
    }

    // Direction: Down
    if (currentY < DimY - gridSize) // Can move down?
    {
        // Move the grid down
        _gameState.GridLocation = new int[] { currentX, currentY + 1 };
        if (IsWinner(GetNextMoveBy()))
        {
            return true; // AI moved grid and won
        }
        // Undo the move
        _gameState.GridLocation = [currentX, currentY];
    }

    // Direction: Left
    if (currentX > 0) // Can move left?
    {
        // Move the grid left
        _gameState.GridLocation = new int[] { currentX - 1, currentY };
        if (IsWinner(GetNextMoveBy()))
        {
            return true; // AI moved grid and won
        }
        // Undo the move
        _gameState.GridLocation = [currentX, currentY];
    }

    // Direction: Right
    if (currentX < DimX - gridSize) // Can move right?
    {
        // Move the grid right
        _gameState.GridLocation = new int[] { currentX + 1, currentY };
        if (IsWinner(GetNextMoveBy()))
        {
            return true; // AI moved grid and won
        }
        // Undo the move
        _gameState.GridLocation = [currentX, currentY];
    }
    // If no winning move was found in any direction, return false
    return false;
}


// Attempts to move an AI piece and check if it results in a win
    private bool TryWinByMovePiece(string aiPlayer)
    {
        for (int fromX = 0; fromX < DimX; fromX++)
        {
            for (int fromY = 0; fromY < DimY; fromY++)
            {
                if (_gameState.GameBoard[fromX][fromY] == aiPlayer)
                {
                    for (int toX = 0; toX < DimX; toX++)
                    {
                        for (int toY = 0; toY < DimY; toY++)
                        {
                            if (_gameState.GameBoard[toX][toY] == _gameState.EmptyCell)
                            {
                                // Make the move and check for a winner
                                if (MovePieceCheckWinner(fromX, fromY, toX, toY))
                                {
                                    return true; // AI moved piece and won
                                }
                                else
                                {   // Undo the move
                                    _gameState.GameBoard[fromX][fromY] = aiPlayer;
                                    _gameState.GameBoard[toX][toY] = _gameState.EmptyCell;
                                    _gameState.NextMoveBy = _gameState.NextMoveBy == GetPlayerX() ? GetPlayerO() : GetPlayerX();
                                }
                                
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    private void MoveAiPieceRandomly(string aiPlayer)
    {
        var aiPieces = new List<(int x, int y)>();
        var emptyCells = new List<(int x, int y)>();

        // Collect all AI pieces and empty cells
        for (int x = 0; x < DimX; x++)
        {
            for (int y = 0; y < DimY; y++)
            {
                if (_gameState.GameBoard[x][y] == aiPlayer)
                {
                    aiPieces.Add((x, y));
                }
                else if (_gameState.GameBoard[x][y] == _gameState.EmptyCell)
                {
                    emptyCells.Add((x, y));
                }
            }
        }

        // Randomly select a game piece and an empty cell
        var random = new Random();
        var (fromX, fromY) = aiPieces[random.Next(aiPieces.Count)];
        var (toX, toY) = emptyCells[random.Next(emptyCells.Count)];

        // Move the selected piece
        _gameState.GameBoard[toX][toY] = aiPlayer;
        _gameState.GameBoard[fromX][fromY] = _gameState.EmptyCell;
        SwitchPlayerIncreaseMovesMade();
    }
}