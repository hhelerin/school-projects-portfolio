using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.Controllers
{
    public class CustomersUsersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CustomersUsers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CustomersUsers.Include(c => c.Customer).Include(c => c.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CustomersUsers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customersUsers = await _context.CustomersUsers
                .Include(c => c.Customer)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customersUsers == null)
            {
                return NotFound();
            }

            return View(customersUsers);
        }

        // GET: CustomersUsers/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CustomersUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Id,UserId")] CustomersUsers customersUsers)
        {
            if (ModelState.IsValid)
            {
                customersUsers.Id = Guid.NewGuid();
                _context.Add(customersUsers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address", customersUsers.CustomerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", customersUsers.UserId);
            return View(customersUsers);
        }

        // GET: CustomersUsers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customersUsers = await _context.CustomersUsers.FindAsync(id);
            if (customersUsers == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address", customersUsers.CustomerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", customersUsers.UserId);
            return View(customersUsers);
        }

        // POST: CustomersUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CustomerId,Id,UserId")] CustomersUsers customersUsers)
        {
            if (id != customersUsers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customersUsers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomersUsersExists(customersUsers.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Address", customersUsers.CustomerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", customersUsers.UserId);
            return View(customersUsers);
        }

        // GET: CustomersUsers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customersUsers = await _context.CustomersUsers
                .Include(c => c.Customer)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customersUsers == null)
            {
                return NotFound();
            }

            return View(customersUsers);
        }

        // POST: CustomersUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customersUsers = await _context.CustomersUsers.FindAsync(id);
            if (customersUsers != null)
            {
                _context.CustomersUsers.Remove(customersUsers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomersUsersExists(Guid id)
        {
            return _context.CustomersUsers.Any(e => e.Id == id);
        }
    }
}
