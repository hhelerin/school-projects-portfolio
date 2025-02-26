using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using GameBrain;

namespace WebApp.Pages.SaveGames
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public SaveGame SaveGame { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savegame = await _context.SaveGames.FirstOrDefaultAsync(m => m.Id == id);
            if (savegame == null)
            {
                return NotFound();
            }
            else
            {
                SaveGame = savegame;
            }
            return Page();
        }
    }
}
