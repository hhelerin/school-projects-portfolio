using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    void SaveConfiguration(GameConfiguration newConfig);
    List<string> GetConfigurationNames();
    GameConfiguration GetConfigurationByName(string name);
    
    GameConfiguration GetConfigById(string configId);
    
}