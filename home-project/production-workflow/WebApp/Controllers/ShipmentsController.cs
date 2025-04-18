using App.DAL.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Domain;
using Base.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class ShipmentsController : Controller
    {
        private readonly IAppUOW _uow;

        public ShipmentsController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: Shipments
        public async Task<IActionResult> Index()
        {
            return View(await _uow.ShipmentRepository.AllAsync());
        }

        // GET: Shipments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _uow.ShipmentRepository.FindAsync(id.Value, User.GetUserId());
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // GET: Shipments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CustomerId"] = new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Address");
            return View();
        }

        // POST: Shipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShippedOn,Method,Details,CustomerId,Id")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                _uow.ShipmentRepository.Add(shipment);
                await _uow.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Address", shipment.CustomerId);
            return View(shipment);
        }

        // GET: Shipments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _uow.ShipmentRepository.FindAsync(id.Value, User.GetUserId());
            if (shipment == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Address", shipment.CustomerId);
            return View(shipment);
        }

        // POST: Shipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ShippedOn,Method,Details,CustomerId,Id")] Shipment shipment)
        {
            if (id != shipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.ShipmentRepository.Update(shipment);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _uow.ShipmentRepository.ExistsAsync(shipment.Id))
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
            ViewData["CustomerId"] = new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Address", shipment.CustomerId);
            return View(shipment);
        }

        // GET: Shipments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _uow.ShipmentRepository.FindAsync(id.Value, User.GetUserId());
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var shipment = await _uow.ShipmentRepository.FindAsync(id);
            if (shipment != null)
            {
                _uow.ShipmentRepository.Remove(shipment);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
