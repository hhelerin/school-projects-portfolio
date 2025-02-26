using System.Data;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryDataBase : IGameRepository
{
    
    static string connectionString = $"Data Source={FileHelper.BasePath}app.db";

    DbContextOptions<AppDbContext> contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;
    

    public string SaveGame(string jsonStateString, string gameConfigName)
    {
        var gameToSave = new SaveGame();
        gameToSave.Name = gameConfigName + " " + 
                          DateTime.Now.ToString("O");
        gameToSave.State = jsonStateString;
        using AppDbContext ctx = new AppDbContext(contextOptions);
        ctx.SaveGames.Add(gameToSave);
        ctx.SaveChanges();
        return gameToSave.Id;
    }

    public string SaveGame(string gameId, string jsonStateString, string gameConfigName)
    {
        using AppDbContext ctx = new AppDbContext(contextOptions);
        
        var existingGame = ctx.SaveGames.FirstOrDefault(g => g.Id == gameId);
        if (existingGame == null)
        {
            throw new ArgumentException($"No game found with ID: {gameId}");
        }
        
        existingGame.Name = gameConfigName + " " + DateTime.Now.ToString("O");
        existingGame.State = jsonStateString;
        
        ctx.SaveChanges();
        return existingGame.Id;
    }

    public TicTacTwoBrain GetGameByName(string getSavedGameName)
    {
        using AppDbContext ctx = new AppDbContext(contextOptions);
        SaveGame saveGame = ctx.SaveGames.FirstOrDefault(x => x.Name == getSavedGameName)
                            ?? throw new KeyNotFoundException($"Game with name '{getSavedGameName}' not found.");
        
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(saveGame.State);
        
        return new TicTacTwoBrain(
            state 
            ?? throw new InvalidOperationException("Deserialization returned null."));
    }

    public List<string> GetSavedGameNames()
    {
        using AppDbContext ctx = new AppDbContext(contextOptions);
        return ctx.SaveGames
            .Select(c => c.Name)   
            .ToList();
    }
    
    public SaveGame LoadGame(string gameId)
    {
        using AppDbContext ctx = new AppDbContext(contextOptions);
        return ctx.SaveGames.FirstOrDefault(x => x.Id == gameId) 
               ?? throw new KeyNotFoundException();
    }

    public List<SaveGame> GetSaveGamesList()
    {
        using AppDbContext ctx = new AppDbContext(contextOptions);
        return new List<SaveGame>(ctx.SaveGames);
    }
}