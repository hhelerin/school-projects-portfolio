namespace BLL;

public class Menu
{
    private string MenuHeader { get; set; }
    private const string MenuDivider = "========================";
    private List<MenuItem> MenuItems { get; set; }
    private bool IsCustomMenu { get; set; }

    private readonly MenuItem _menuItemExit = new()
    {
        Shortcut = "E",
        Title = "Exit"
    };
    
    private readonly MenuItem _menuItemReturn = new MenuItem()
    {
        Shortcut = "R",
        Title = "Return"
    };
    private readonly MenuItem _menuItemReturnMain = new MenuItem()
    {
        Shortcut = "M",
        Title = "Return to Main menu"
    };

    private EMenuLevel MenuLevel { get; set; }
    
    public void SetMenuItemAction(string shortCut, Func<string> action)
    {
        var menuItem = MenuItems.Single(m => m.Shortcut == shortCut);
        menuItem.MenuItemAction = action;
    }


    public Menu(EMenuLevel menuLevel, string menuHeader, List<MenuItem> menuItems, bool isCustomMenu = false)
    {
        if (string.IsNullOrWhiteSpace(menuHeader))
        {
            throw new ApplicationException("Menu header cannot be empty.");
        }

        MenuHeader = menuHeader;

        if (menuItems == null || menuItems.Count == 0)
        {
            throw new ApplicationException("Menu items cannot be empty.");
        }

        MenuItems = menuItems;
        MenuLevel = menuLevel;
        IsCustomMenu = isCustomMenu;
        
        
        if (MenuLevel != EMenuLevel.Main)
        {
            MenuItems.Add(_menuItemReturn);
        }
        
        if (MenuLevel == EMenuLevel.Ternary)
        {
            MenuItems.Add(_menuItemReturnMain);
        }

        MenuItems.Add(_menuItemExit);
        
        List<string> menuItemShortcuts = menuItems.Select(mi => mi.Shortcut).ToList();
 
        if(menuItemShortcuts.GroupBy(s => s)
           .Any(g => g.Count() > 1))
        {
            foreach (string s in menuItemShortcuts)
            {
                Console.WriteLine(s);
            }
            throw new ApplicationException("Menu item shortcuts should be unique.");
        }
        
    }

    public string Run()
    {
        Console.Clear();

        do
        {
            var menuItem = DisplayMenuGetUserChoice();
            var menuReturnValue = "";
        

            if (menuItem.MenuItemAction != null)
            {
                menuReturnValue = menuItem.MenuItemAction();
                if (IsCustomMenu) return menuReturnValue;
            }

            if (menuItem.Shortcut == _menuItemReturn.Shortcut)
            {
                return menuItem.Shortcut;
            }
        
            if (menuItem.Shortcut == _menuItemExit.Shortcut || menuReturnValue == _menuItemExit.Shortcut) 
            {
                return _menuItemExit.Shortcut;
            }
            
            if ((menuItem.Shortcut == _menuItemReturnMain.Shortcut || menuReturnValue == _menuItemReturnMain.Shortcut) && MenuLevel != EMenuLevel.Main)
            {
                return _menuItemReturnMain.Shortcut;
            }
            
        } while (true);
        
    }

    private MenuItem DisplayMenuGetUserChoice()
    {
        do
        {
            DrawMenu();

            var userInput = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("To continue, just write out the option's symbol and press Enter!");
                Console.WriteLine("For example, to select Exit, write e and press Enter key");
                Console.WriteLine();
            }
            else
            {
                userInput = userInput.ToUpper();
                
                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;
                    return menuItem;
                }

                Console.WriteLine("There's no such option :(");
                
                Console.WriteLine("For example, to select Exit, write e and press Enter key");
                Console.WriteLine();
            }
        } while (true);
    }

    private void DrawMenu()
    {
        Console.WriteLine(MenuHeader);
        Console.WriteLine(MenuDivider);

        foreach (var menuItem in MenuItems)
        {
            Console.WriteLine(menuItem);
        }
        
        Console.Write(MenuDivider);
        Console.WriteLine();
        Console.Write("Enter the option's symbol to select...");
        Console.WriteLine();
        Console.Write(">>");
    }
    
}