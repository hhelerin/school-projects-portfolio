using GameBrain;

namespace DAL;

public class ConfigRepositoryJson: IConfigRepository

{
    private const string Extension = ".config.json";
    
    private static readonly string BasePath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
        + System.IO.Path.DirectorySeparatorChar + "tic-tac-two" + System.IO.Path.DirectorySeparatorChar;

    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();
        return System.IO.Directory
            .GetFiles(BasePath, "*" + Extension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName)))
            .ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var configJsonStr = System.IO.File.ReadAllText(BasePath + name + Extension);
        return System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(configJsonStr) 
               ?? throw new InvalidOperationException();
    }

    public GameConfiguration GetConfigById(string configId)
    {
        var gameConfigurations = 
            GetConfigurationNames()
            .Select(name =>GetConfigurationByName(name))
            .ToList();
        
        return gameConfigurations.FirstOrDefault(conf => conf.Id == configId) 
               ?? throw new KeyNotFoundException();
    
    }


    public void SaveConfiguration(GameConfiguration newConfig)
    {
        var configJsonStr = System.Text.Json.JsonSerializer.Serialize(newConfig);
        System.IO.File.WriteAllText(BasePath + newConfig.Name + Extension, configJsonStr);
    }
    

    private void CheckAndCreateInitialConfig()
    {
        if (!Directory.Exists(BasePath))
        {
            System.IO.Directory.CreateDirectory(BasePath);
        }
        
        var data = System.IO.Directory.GetFiles
            (BasePath, "*" + Extension).ToList();
        if (data.Count != 0) return;
        
        var hardCodedRepo = new ConfigRepositoryHardCoded();
        var optionNames = hardCodedRepo.GetConfigurationNames();
        foreach (var optionName in optionNames)
        {
            var gameOption = hardCodedRepo.GetConfigurationByName(optionName);
            SaveConfiguration(gameOption);
        }
    }
}