using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using GameBrain;

namespace WebApp.Pages.GameConfigurations
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GameConfiguration GameConfiguration { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameconfiguration = await _context.GameConfigurations.FirstOrDefaultAsync(m => m.Id == id);

            if (gameconfiguration == null)
            {
                return NotFound();
            }
            else
            {
                GameConfiguration = gameconfiguration;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameconfiguration = await _context.GameConfigurations.FindAsync(id);
            if (gameconfiguration != null)
            {
                GameConfiguration = gameconfiguration;
                _context.GameConfigurations.Remove(GameConfiguration);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
