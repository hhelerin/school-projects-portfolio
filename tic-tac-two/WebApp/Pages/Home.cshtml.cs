using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class Home : PageModel

{
    [BindProperty(SupportsGet = true)]
    public string UserName { get; set; } = default!;
    [BindProperty]
    public string SelectedGameType { get; set; } = default!;

    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;

    public Home(IGameRepository gameRepository, AppDbContext context, IConfigRepository configRepository)
    {
        _gameRepository = gameRepository;
        _configRepository = configRepository;
    }

    public List<SaveGame> SaveGames { get; set; } = new List<SaveGame>();
    [BindProperty]
    public List<TicTacTwoBrain> Games { get; set; } = new List<TicTacTwoBrain>();
    public List<string> GameNames { get; set; } = new List<string>();
    public List<GameConfiguration> GameConfigurations { get; set; } = default!;
    [BindProperty]
    public string? SelectedConfigId { get; set; }

    public string? User2Name { get; set; }


    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(UserName)) 
            return RedirectToPage("./Index", new { error = "No username provided." });
        if (UserName == "X" || UserName == "O" || UserName == "AI" || UserName == "AI_X" || UserName == "AI_O") 
            return RedirectToPage("./Index", new { error = "Choose a different user name." });
        ViewData["UserName"] = UserName;
        
        GameConfigurations = _configRepository
            .GetConfigurationNames()
            .Select(name =>_configRepository.GetConfigurationByName(name))
            .ToList();
        
        SaveGames = _gameRepository.GetSaveGamesList();
        
        GameNames = _gameRepository
            .GetSavedGameNames();
        
        Games = _gameRepository
            .GetSavedGameNames()
            .Select(name => _gameRepository.GetGameByName(name))
            .ToList();
            
        
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(SelectedConfigId))
        {
            ModelState.AddModelError(string.Empty, "Please select a configuration.");
            return Page();
        }

        if (string.IsNullOrEmpty(SelectedGameType))
        {
            ModelState.AddModelError(string.Empty, "Please select a game type.");
            return Page();
        }

        // Retrieve the selected GameConfiguration
        var selectedConfig = _configRepository.GetConfigById(SelectedConfigId);

        // Convert the selected game type to the enum
        if (!Enum.TryParse<EGameType>(SelectedGameType, out var gameType))
        {
            ModelState.AddModelError(string.Empty, "Invalid game type selected.");
            return Page();
        }

        var newGame = new TicTacTwoBrain(selectedConfig);

        newGame.SetGameType(gameType);
        newGame.SetPlayerX(UserName);
        newGame.SetSaveGameId(Guid.NewGuid().ToString());

        // Set the AI or player configurations based on game type
        if (gameType == EGameType.PlayerVersusAI)
        {
            newGame.SetPlayerO("AI");
        }
        newGame.SetNextMoveBy(UserName);
        
        if (gameType == EGameType.AIVersusAI)
        {
            newGame.SetPlayerX("AI_X");
            newGame.SetPlayerO("AI_O");
            newGame.SetNextMoveBy("AI_X");
        }

        var saveGameId = _gameRepository.SaveGame(newGame.GetGameStateJson(), selectedConfig.Name);
        
        return RedirectToPage("/PlayGame/SelectAction", new { gameid = saveGameId, UserName });
    }

}
    
    
