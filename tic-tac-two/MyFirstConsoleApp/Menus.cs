using BLL;

namespace MyFirstConsoleApp;

public static class Menus
{
    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = GameSetupHelper.ChooseConfigForNewGame
            }, 
            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load game",
                MenuItemAction = GameSetupHelper.LoadSavedGame
            }
        ]);
}