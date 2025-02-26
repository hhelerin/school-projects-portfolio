using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        Console.WriteLine();
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                string[][] board = gameInstance.GetBoard();

                if (x >= gameInstance.GetGridLocation()[0]
                    && x < (gameInstance.GetGridLocation()[0] + gameInstance.GetGridSize())
                    && y >= gameInstance.GetGridLocation()[1] 
                    && y < (gameInstance.GetGridLocation()[1] + gameInstance.GetGridSize()))
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                
                
                string currentPiece = board[x][y];
                if (currentPiece == "Empty")
                {
                        Console.Write("     ");
                        Console.ResetColor();
                }
                else
                {
                    if (currentPiece == "X" || currentPiece == gameInstance.GetPlayerX())
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("  " + "X" + "  ");
                        Console.ResetColor();
                    }else if (currentPiece == "O" || currentPiece == gameInstance.GetPlayerO())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("  " + "O" + "  ");
                        Console.ResetColor();
                    }
                }
                if (x == gameInstance.DimX - 1) continue;
                Console.Write("|");
            }
            Console.WriteLine();
            if (y == gameInstance.DimY - 1) continue;
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write("-----");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("+");
                }
            }
            Console.WriteLine();
        }
    }
    
    
    public static void DrawBoardWithCursor(TicTacTwoBrain gameInstance, int cursorX, int cursorY)
    {
        var board = gameInstance.GetBoard();
        int gridSize = gameInstance.GetGridSize();
        int gridStartX = gameInstance.GetGridLocation()[0];
        int gridStartY = gameInstance.GetGridLocation()[1];
        
        for (int y = 0; y < gameInstance.DimY; y++)
        {
            for (int x = 0; x < gameInstance.DimX; x++)
            {
                if (x == cursorX && y == cursorY)
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                }
                else if (x >= gridStartX && x < gridStartX + gridSize && y >= gridStartY && y < gridStartY + gridSize)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ResetColor();
                }
                
                string content = 
                    board[x][y] == "Empty" ? "     " : 
                    board[x][y] == "X" || board[x][y]== gameInstance.GetPlayerX() ? "  X  " : "  O  ";
                
                Console.ForegroundColor = board[x][y] == "X" ? ConsoleColor.Blue 
                    : board[x][y] == "O" ? ConsoleColor.Red : ConsoleColor.Gray;

                Console.Write(content);
                Console.ResetColor();
                
                if (x < gameInstance.DimX - 1) Console.Write("|");
            }
            Console.WriteLine();
            
            if (y < gameInstance.DimY - 1)
            {
                for (int x = 0; x < gameInstance.DimX; x++)
                {
                    Console.Write("-----");
                    if (x < gameInstance.DimX - 1) Console.Write("+");
                }
                Console.WriteLine();
                
            }
        }Console.WriteLine("Use arrow keys to move and Enter to select.");
    }
}
