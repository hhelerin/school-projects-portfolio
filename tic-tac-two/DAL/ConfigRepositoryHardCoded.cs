using GameBrain;

namespace DAL;

public class ConfigRepositoryHardCoded: IConfigRepository
{
    private static List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
    {
        new GameConfiguration()
        {
            Name = "Classical",
        },
        new GameConfiguration()
        {
            Name = "Big Board",
            BoardWidth = 8,
            BoardHeight = 8,
            WinCondition = 4,
            MovePieceOrGridAfterNMoves = 3,
            PieceCount = 12,
            MovesPerGame = 1000,
            GridSize = 4
        },
        new GameConfiguration()
        {
        Name = "Tic-Tac-Toe",
        BoardWidth = 3, 
        BoardHeight = 3,
        WinCondition = 3,
        MovePieceOrGridAfterNMoves = -1,
        PieceCount = 5,
        MovesPerGame = 18
    }
    };
    
    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations.
            OrderBy(x => x.Name).
            Select(config => config.Name).
            ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        return _gameConfigurations.SingleOrDefault(x => x.Name == name) 
               ?? throw new InvalidOperationException();
    }

    public GameConfiguration GetConfigById(string configId)
    {
        throw new NotImplementedException();
    }

    public void SaveConfiguration(GameConfiguration gameConfiguration)
    {
        _gameConfigurations.Add(gameConfiguration);
    }

}