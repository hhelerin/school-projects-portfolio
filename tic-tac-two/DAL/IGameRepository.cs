using GameBrain;

namespace DAL;

public interface IGameRepository
{
    string SaveGame(string jsonStateString, string gameConfigName);
    
    string SaveGame(string gameId, string jsonStateString, string gameConfigName);
    TicTacTwoBrain GetGameByName(string savedGameName);
    List<string> GetSavedGameNames();

    SaveGame LoadGame(string gameId);
    List<SaveGame> GetSaveGamesList();
}