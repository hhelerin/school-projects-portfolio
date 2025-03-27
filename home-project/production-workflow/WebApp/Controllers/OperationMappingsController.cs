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
    public class OperationMappingsController : Controller
    {
        private readonly AppDbContext _context;

        public OperationMappingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OperationMappings
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.OperationMappings.Include(o => o.Order).Include(o => o.ProcessingStep);
            return View(await appDbContext.ToListAsync());
        }

        // GET: OperationMappings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operationMapping = await _context.OperationMappings
                .Include(o => o.Order)
                .Include(o => o.ProcessingStep)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (operationMapping == null)
            {
                return NotFound();
            }

            return View(operationMapping);
        }

        // GET: OperationMappings/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Name");
            ViewData["ProcessingStepId"] = new SelectList(_context.ProcessingSteps, "Id", "Name");
            return View();
        }

        // POST: OperationMappings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProcessingStepId,OrderId,PrerequisitesObtained,CompletedAt,Details,Id")] OperationMapping operationMapping)
        {
            if (ModelState.IsValid)
            {
                operationMapping.Id = Guid.NewGuid();
                _context.Add(operationMapping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Name", operationMapping.OrderId);
            ViewData["ProcessingStepId"] = new SelectList(_context.ProcessingSteps, "Id", "Name", operationMapping.ProcessingStepId);
            return View(operationMapping);
        }

        // GET: OperationMappings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operationMapping = await _context.OperationMappings.FindAsync(id);
            if (operationMapping == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Name", operationMapping.OrderId);
            ViewData["ProcessingStepId"] = new SelectList(_context.ProcessingSteps, "Id", "Name", operationMapping.ProcessingStepId);
            return View(operationMapping);
        }

        // POST: OperationMappings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProcessingStepId,OrderId,PrerequisitesObtained,CompletedAt,Details,Id")] OperationMapping operationMapping)
        {
            if (id != operationMapping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(operationMapping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OperationMappingExists(operationMapping.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Name", operationMapping.OrderId);
            ViewData["ProcessingStepId"] = new SelectList(_context.ProcessingSteps, "Id", "Name", operationMapping.ProcessingStepId);
            return View(operationMapping);
        }

        // GET: OperationMappings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operationMapping = await _context.OperationMappings
                .Include(o => o.Order)
                .Include(o => o.ProcessingStep)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (operationMapping == null)
            {
                return NotFound();
            }

            return View(operationMapping);
        }

        // POST: OperationMappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var operationMapping = await _context.OperationMappings.FindAsync(id);
            if (operationMapping != null)
            {
                _context.OperationMappings.Remove(operationMapping);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OperationMappingExists(Guid id)
        {
            return _context.OperationMappings.Any(e => e.Id == id);
        }
    }
}
