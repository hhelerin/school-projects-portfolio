using GameBrain;
using Microsoft.EntityFrameworkCore;
using InvalidOperationException = System.InvalidOperationException;

namespace DAL;

public class ConfigRepositoryDataBase : IConfigRepository
{
    private static readonly string ConnectionString = $"Data Source={FileHelper.BasePath}app.db";

    private readonly DbContextOptions<AppDbContext> _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(ConnectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .Options;

    public void SaveConfiguration(GameConfiguration newConfig)
    {
        using AppDbContext ctx = new AppDbContext(_contextOptions);
        ctx.GameConfigurations.Add(newConfig);
        ctx.SaveChanges();
    }

    public List<string> GetConfigurationNames()
    {
        using AppDbContext ctx = new AppDbContext(_contextOptions);
        return ctx.GameConfigurations
            .Select(c => c.Name)   
            .ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        using AppDbContext ctx = new AppDbContext(_contextOptions);
        return ctx.GameConfigurations
            .FirstOrDefault(config => config.Name == name) 
               ?? throw new KeyNotFoundException();
    }

    
    public GameConfiguration GetConfigById(string configId)
    {
        using AppDbContext ctx = new AppDbContext(_contextOptions);
        return ctx.GameConfigurations.FirstOrDefault(c => c.Id == configId) 
               ?? throw new InvalidOperationException();
    }

    private void CheckAndCreateInitialConfig()
    {
        using AppDbContext ctx = new AppDbContext(_contextOptions);
        if (ctx.GameConfigurations.Any()) return;
        var hardCodedRepo = new ConfigRepositoryHardCoded();
        var optionNames = hardCodedRepo.GetConfigurationNames();
        foreach (var optionName in optionNames)
        {
            var gameOption = hardCodedRepo.GetConfigurationByName(optionName);
            ctx.GameConfigurations.Add(gameOption);
        }
        ctx.SaveChanges();
    }
}