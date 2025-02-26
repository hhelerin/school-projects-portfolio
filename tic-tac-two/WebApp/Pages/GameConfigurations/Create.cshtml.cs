using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using GameBrain;

namespace WebApp.Pages.GameConfigurations
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)] public string? UserName { get; set; }
        public IActionResult OnGet()
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                GameConfiguration = new GameConfiguration
                {
                    Name = $"{UserName}{RandomNumberGenerator.GetInt32(100, 9999)}"
                };
            }
            return Page();
        }

        [BindProperty]
        public GameConfiguration GameConfiguration { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (GameConfiguration.GridSize > Math.Min(GameConfiguration.BoardWidth, GameConfiguration.BoardHeight))
            {
                ModelState.AddModelError("GameConfiguration.GridSize",
                    $"Grid Size must not exceed the smaller of Board Width ({GameConfiguration.BoardWidth}) " +
                    $"or Board Height ({GameConfiguration.BoardHeight}).");
            }
            
            if (GameConfiguration.MovesPerGame < (4 * GameConfiguration.PieceCount))
            {
                ModelState.AddModelError("GameConfiguration.MovesPerGame",
                    $"Moves per Game must be at least 4 times the Pieces Per Player. " +
                    $"({GameConfiguration.PieceCount})");
            }

            if (GameConfiguration.WinCondition > GameConfiguration.GridSize)
            {
                ModelState.AddModelError("GameConfiguration.WinCondition",
                    "Win Condition must not exceed Grid Size.");
            }

            if (GameConfiguration.PieceCount < GameConfiguration.WinCondition)
            {
                ModelState.AddModelError("GameConfiguration.PieceCount",
                    "Piece Count must be at least equal to the Win Condition.");
            }

            if (GameConfiguration.MovesPerGame < GameConfiguration.PieceCount * 4)
            {
                ModelState.AddModelError("GameConfiguration.MovesPerGame",
                    $"Moves Per Game must be at least 20 " +
                    "(Suggested 100 or more moves).");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.GameConfigurations.Add(GameConfiguration);
            await _context.SaveChangesAsync();
            
            if (!string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("/Home", new {UserName});
            }
            ;

            return RedirectToPage("./Index");
        }
    }
}
