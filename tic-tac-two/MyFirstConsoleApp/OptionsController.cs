using System.Security.Cryptography;
using DAL;
using GameBrain;

namespace MyFirstConsoleApp;

public static class OptionsController
{
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryDataBase();
    public static GameConfiguration CreateConfig()
        {
        var config = new GameConfiguration();
        
        Console.WriteLine("Enter your name:");
        config.Name = Console.ReadLine() + RandomNumberGenerator.GetInt32(100, 9999);
        
        Console.Clear();
        config.BoardWidth = GetValidInput("Enter Board Width (3-13):", minValue: 3, maxValue: 13);
        
        Console.Clear();
        config.BoardHeight = GetValidInput("Enter Board Height (3-13):", minValue: 3, maxValue: 13);
        
        Console.Clear();
        config.GridSize = GetValidInput(
            $"Enter Inner Grid Size (must fit in the gameboard. Min 3, " +
            $"max {Math.Min(config.BoardWidth, config.BoardHeight)}):",
            minValue: 3
        );
        Console.Clear();
        config.WinCondition = GetValidInput("Enter Winning Condition (number of pieces in a row to win):", 
            minValue: 3, maxValue: config.GridSize);
        
        Console.Clear();
        config.PieceCount = GetValidInput(
            $"Enter Number Of Marks Per Player (must be at least {config.WinCondition}):", 
            minValue: config.WinCondition
        );
        Console.Clear();
        config.MovesPerGame = GetValidInput(
            $"Enter Total Moves Per Game (must be at least 20, suggested 100 or more moves.):", 
            minValue: 20
        );
        Console.Clear();
        config.MovePieceOrGridAfterNMoves = 
            GetValidInput("Enter number of moves after which the piece or grid can be moved (-1 to disable):", 
                minValue: -1);

        Console.Clear();
        int saveConfig =
            GetValidInput("Do you want to save this configuration? (1 to save, 2 to discard after gameover):",
                minValue: 1, maxValue: 2);
        if (saveConfig == 1)
        {
            ConfigRepository.SaveConfiguration(config);
        }
        return config;
    }
    
    private static int GetValidInput(string prompt, int minValue, int maxValue = int.MaxValue)
    {
        int value;
        bool isValid;

        do
        {
            Console.WriteLine(prompt);
            string? input = Console.ReadLine();

            isValid = int.TryParse(input, out value) && value >= minValue && value <= maxValue;

            if (!isValid)
            {
                Console.Clear();
                Console.WriteLine($"Invalid input! Please enter a number between {minValue} and {maxValue}.");
            }

        } while (!isValid);
        return value;
    }
}