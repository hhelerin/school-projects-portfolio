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
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<GameConfiguration> GameConfiguration { get;set; } = default!;

        public async Task OnGetAsync()
        {
            GameConfiguration = await _context.GameConfigurations.ToListAsync();
        }
    }
}
