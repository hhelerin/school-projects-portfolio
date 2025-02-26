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
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savegame = await _context.SaveGames.FindAsync(id);
            if (savegame != null)
            {
                SaveGame = savegame;
                _context.SaveGames.Remove(SaveGame);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
