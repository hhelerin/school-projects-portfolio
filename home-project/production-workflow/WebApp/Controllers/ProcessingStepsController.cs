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
    public class ProcessingStepsController : Controller
    {
        private readonly AppDbContext _context;

        public ProcessingStepsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProcessingSteps
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProcessingSteps.ToListAsync());
        }

        // GET: ProcessingSteps/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processingStep = await _context.ProcessingSteps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (processingStep == null)
            {
                return NotFound();
            }

            return View(processingStep);
        }

        // GET: ProcessingSteps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProcessingSteps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Details,Id")] ProcessingStep processingStep)
        {
            if (ModelState.IsValid)
            {
                processingStep.Id = Guid.NewGuid();
                _context.Add(processingStep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(processingStep);
        }

        // GET: ProcessingSteps/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processingStep = await _context.ProcessingSteps.FindAsync(id);
            if (processingStep == null)
            {
                return NotFound();
            }
            return View(processingStep);
        }

        // POST: ProcessingSteps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Details,Id")] ProcessingStep processingStep)
        {
            if (id != processingStep.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(processingStep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessingStepExists(processingStep.Id))
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
            return View(processingStep);
        }

        // GET: ProcessingSteps/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processingStep = await _context.ProcessingSteps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (processingStep == null)
            {
                return NotFound();
            }

            return View(processingStep);
        }

        // POST: ProcessingSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var processingStep = await _context.ProcessingSteps.FindAsync(id);
            if (processingStep != null)
            {
                _context.ProcessingSteps.Remove(processingStep);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcessingStepExists(Guid id)
        {
            return _context.ProcessingSteps.Any(e => e.Id == id);
        }
    }
}
