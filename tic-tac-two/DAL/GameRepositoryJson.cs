using GameBrain;
using InvalidOperationException = System.InvalidOperationException;
using NotImplementedException = System.NotImplementedException;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    private const string Extension = ".game.json";
    
    private static readonly string BasePath = 
        Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
        + System.IO.Path.DirectorySeparatorChar + "tic-tac-two" + System.IO.Path.DirectorySeparatorChar;
    public string SaveGame(string jsonStateString, string gameConfigName)
    {
        var fileName = FileHelper.BasePath + 
                       gameConfigName + " " + 
                       DateTime.Now.ToString("O") + 
                       FileHelper.GameExtension;
        
        System.IO.File.WriteAllText(fileName, jsonStateString);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(jsonStateString) 
                    ?? throw new InvalidOperationException();
        
        return state.SaveGameId ?? throw new InvalidOperationException();
    }

    public string SaveGame(string gameId, string jsonStateString, string gameConfigName)
    {
        var gameFiles = System.IO.Directory.GetFiles(BasePath, "*" + Extension);

        foreach (var file in gameFiles)
        {
            var fileContent = System.IO.File.ReadAllText(file);

            var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(fileContent);

            if (state == null) continue;

            if (state.SaveGameId == gameId)
            {
                System.IO.File.WriteAllText(file, jsonStateString);

                return gameId;
            }
        }

        throw new InvalidOperationException($"Game with ID '{gameId}' not found.");
    }


    public TicTacTwoBrain GetGameByName(string name)
    {
        var gameJsonStr = System.IO.File.ReadAllText(BasePath + name + Extension);
        var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(gameJsonStr) 
                    ?? throw new InvalidOperationException();
        return new TicTacTwoBrain(state);
    }

    public List<string> GetSavedGameNames()
    {
        return System.IO.Directory
            .GetFiles(BasePath, "*" + Extension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullFileName)))
            .ToList();
    }

    public SaveGame LoadGame(string gameId)
    {
        var gameFiles = System.IO.Directory.GetFiles(BasePath, "*" + Extension);

        foreach (var file in gameFiles)
        {
            var fileContent = System.IO.File.ReadAllText(file);

            var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(fileContent);

            if (state == null) continue;

            if (state.SaveGameId == gameId)
            {
                var saveGame = new SaveGame();
                saveGame.Name = file;
                saveGame.State = fileContent;
                // Step 3: Return the matching GameState
                return saveGame;
            }
        }
        throw new InvalidOperationException($"Game with ID '{gameId}' not found.");
    }

    public List<SaveGame> GetSaveGamesList()
    {
        var saveGames = new List<SaveGame>();

        var gameFiles = System.IO.Directory.GetFiles(BasePath, "*" + Extension);

        // Step 2: Process each file
        foreach (var file in gameFiles)
        {
            try
            {
                var fileContent = System.IO.File.ReadAllText(file);

                var saveGame = new SaveGame
                {
                    Name = Path.GetFileNameWithoutExtension(file), // Use the file name without the extension
                    State = fileContent // Store the raw JSON state as the state string
                };
                var state = System.Text.Json.JsonSerializer.Deserialize<GameState>(fileContent);
                if (state != null) saveGame.Id = state.SaveGameId ?? throw new InvalidDataException();

                saveGames.Add(saveGame);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{file}': {ex.Message}");
            }
        }
        return saveGames;
    }
}