using App.DAL.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.DTO;
using Base.Helpers;
using Microsoft.AspNetCore.Authorization;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class OperationMappingsController : Controller
    {
        private readonly IAppUOW _uow;

        public OperationMappingsController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: OperationMappings
        public async Task<IActionResult> Index()
        {
            var operationMappings = _uow.OperationMappingRepository.AllAsync();
            return View(await operationMappings);
        }

        // GET: OperationMappings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _uow.OperationMappingRepository.FindAsync(id.Value, User.GetUserId());


            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }


// GET: OperationMappings/Create
        public async Task<IActionResult> Create()
        {
            var vm = new OperationMappingCreateEditViewModel
            {
                OperationMapping = new OperationMappingDto(),
                OrderSelectList = new SelectList(await _uow.OrderRepository.AllAsync(), "Id", "Name"),
                ProcessingStepSelectList = new SelectList(await _uow.ProcessingStepRepository.AllAsync(), "Id", "Name")
            };

            return View(vm);
        }

        // POST: OperationMappings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperationMappingCreateEditViewModel vm)
        {   
            if (ModelState.IsValid)
            {
                vm.OperationMapping.Id = Guid.NewGuid(); // Or let DB assign if configured
                _uow.OperationMappingRepository.Add(vm.OperationMapping);
                await _uow.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.OrderSelectList = new SelectList(await _uow.OrderRepository.AllAsync(), "Id", "Name", vm.OperationMapping.OrderId);
            vm.ProcessingStepSelectList = new SelectList(await _uow.ProcessingStepRepository.AllAsync(), "Id", "Name", vm.OperationMapping.ProcessingStepId);

            return View(vm);
        }

        // GET: OperationMappings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var entity = await _uow.OperationMappingRepository.FindAsync(id.Value, User.GetUserId());
            if (entity == null) return NotFound();

            var vm = new OperationMappingCreateEditViewModel
            {
                OperationMapping = entity,
                OrderSelectList = new SelectList(await _uow.OrderRepository.AllAsync(), "Id", "Name", entity.OrderId),
                ProcessingStepSelectList = new SelectList(await _uow.ProcessingStepRepository.AllAsync(), "Id", "Name", entity.ProcessingStepId)
            };

            return View(vm);
        }

        // POST: OperationMappings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, OperationMappingCreateEditViewModel vm)
        {
            if (id != vm.OperationMapping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.OperationMappingRepository.Update(vm.OperationMapping);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _uow.OperationMappingRepository.ExistsAsync(vm.OperationMapping.Id))
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

    
            vm.OrderSelectList = new SelectList(await _uow.OrderRepository.AllAsync(), "Id", "Name", vm.OperationMapping.OrderId);
            vm.ProcessingStepSelectList = new SelectList(await _uow.ProcessingStepRepository.AllAsync(), "Id", "Name", vm.OperationMapping.ProcessingStepId);

            return View(vm);
        }

        // GET: OperationMappings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operationMapping = await _uow.OperationMappingRepository.FindAsync(id.Value, User.GetUserId());
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
            var operationMapping = await _uow.OperationMappingRepository.FindAsync(id);
            if (operationMapping != null)
            {
                _uow.OperationMappingRepository.Remove(operationMapping);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}