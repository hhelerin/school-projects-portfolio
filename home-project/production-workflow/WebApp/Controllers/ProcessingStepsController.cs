using App.DAL.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.DTO;
using Base.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class ProcessingStepsController : Controller
    {
        private readonly IAppUOW _uow;

        public ProcessingStepsController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: ProcessingSteps
        public async Task<IActionResult> Index()
        {
            return View(await _uow.ProcessingStepRepository.AllAsync());
        }

        // GET: ProcessingSteps/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var processingStep = await _uow.ProcessingStepRepository.FindAsync(id.Value, User.GetUserId());
            
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
        public async Task<IActionResult> Create([Bind("Name,Details,Id")] ProcessingStepDto processingStep)
        {
            if (ModelState.IsValid)
            {
                processingStep.Id = Guid.NewGuid();
                _uow.ProcessingStepRepository.Add(processingStep);
                await _uow.SaveChangesAsync();
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

            var processingStep = await _uow.ProcessingStepRepository.FindAsync(id.Value, User.GetUserId());
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Details,Id")] ProcessingStepDto processingStep)
        {
            if (id != processingStep.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.ProcessingStepRepository.Update(processingStep);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _uow.ProcessingStepRepository.ExistsAsync(processingStep.Id))
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

            var processingStep = await _uow.ProcessingStepRepository.FindAsync(id.Value, User.GetUserId());
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
            var processingStep = await _uow.ProcessingStepRepository.FindAsync(id);
            if (processingStep != null)
            {
                _uow.ProcessingStepRepository.Remove(processingStep);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
