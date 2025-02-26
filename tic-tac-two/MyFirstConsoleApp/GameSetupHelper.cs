using BLL;
using DAL;
using GameBrain;

namespace MyFirstConsoleApp;

public static class GameSetupHelper
{
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryDataBase();
    private static readonly IGameRepository GameRepository = new GameRepositoryDataBase();

    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < ConfigRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = ConfigRepository.GetConfigurationNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        configMenuItems.Add(new MenuItem()
        {
            Title = "Create Custom Game",
            Shortcut = "C",
            MenuItemAction = () => "Custom"
        });

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game config",
            configMenuItems,
            isCustomMenu: true
        );
        return configMenu.Run();
    }
    
    public static string ChooseConfigForNewGame()
    {
        GameConfiguration chosenConfig;
        var chosenConfigShortcut = ChooseConfiguration();

        bool isNumber = int.TryParse(chosenConfigShortcut, out var configNumber);
        
        if (!isNumber)
        {
            if (chosenConfigShortcut == "Custom")
            {
                Console.Clear();
                Console.WriteLine("You're about to create a custom game experience.");
                chosenConfig = OptionsController.CreateConfig();
            }else
            {
                var s = chosenConfigShortcut;
                return s;
            }
        }
        else
        {
            List<string> configNames = ConfigRepository.GetConfigurationNames();
            string chosenConfigName = configNames[configNumber];
            chosenConfig = ConfigRepository.GetConfigurationByName(chosenConfigName);
        }

        var gameInstance = new TicTacTwoBrain(chosenConfig);
        Console.WriteLine("...STARTING GAME...");
        Console.WriteLine("What's your username?");
        var username = Console.ReadLine();
        if (username != null)
        {
            gameInstance.SetPlayerX(username);
            gameInstance.SetNextMoveBy(username);
        }
        Console.WriteLine("What's your opponent's username?");
        var user2Name = Console.ReadLine();
        if (user2Name != null) gameInstance.SetPlayerO(user2Name);
        GameController.MainLoop(gameInstance);
        return "";
    }
    
    public static string LoadSavedGame()
    {
        var chosenGameShortcut = ChooseSavedGame();

        bool isNumber = int.TryParse(chosenGameShortcut, out var gameNumber);
        
        if (!isNumber)
        {
            return chosenGameShortcut;
        }
        
        var chosenGame = GameRepository.GetGameByName(
            GameRepository.GetSavedGameNames()[gameNumber]
        );
        
        GameController.MainLoop(chosenGame);
        return "";
    }

    private static string ChooseSavedGame()
    {
        var savedGames = new List<MenuItem>();
        if (GameRepository.GetSavedGameNames().Count == 0)
        {
            Console.Clear();
            Console.WriteLine("There are no saved games yet. Play a new game!");
            return "";
        }

        for (var i = 0; i < GameRepository.GetSavedGameNames().Count; i++)
        {
            var game = GameRepository.GetGameByName(GameRepository.GetSavedGameNames()[i]);
            if (game.GetStatus() == EGameStatus.Ongoing)
            {
                var returnValue = i.ToString();
                savedGames.Add(new MenuItem()
                {
                    Title = GameRepository.GetSavedGameNames()[i],
                    Shortcut = (i + 1).ToString(),
                    MenuItemAction = () => returnValue
                
                });
            }

        }

        var loadGameMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - load saved game",
            savedGames,
            isCustomMenu: true
        );
        return loadGameMenu.Run();
    }
}