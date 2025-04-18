using App.DAL.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IAppUOW _uow;

        public OrdersController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _uow.OrderRepository
                .AllAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var order = await _uow.OrderRepository.FindAsync(id.Value); 
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CustomerId"] = new SelectList(await _uow.CustomerRepository.AllAsync(), "Id", "Address");
            ViewData["MaterialId"] = new SelectList(await _uow.MaterialRepository.AllAsync(), "Id", "Name");
            ViewData["ShipmentID"] = new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Method");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "CustomerId,MaterialId,OrderNumber,Name,OrderDate,Deadline,TotalAmount,TotalArea,LinearMeter,Details,Status,ShipmentID,PalletNumber,BillingDate,Id")]
            Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid();
                _uow.OrderRepository.Add(order);
                await _uow.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] =
                new SelectList(await _uow.CustomerRepository.AllAsync(), "Id", "Address", order.CustomerId);
            ViewData["MaterialId"] =
                new SelectList(await _uow.MaterialRepository.AllAsync(), "Id", "Name", order.MaterialId);
            ViewData["ShipmentID"] =
                new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Method", order.ShipmentID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var order = await _uow.OrderRepository.FindAsync(id.Value);
            if (order == null) return NotFound();

            ViewData["CustomerId"] =
                new SelectList(await _uow.CustomerRepository.AllAsync(), "Id", "Address", order.CustomerId);
            ViewData["MaterialId"] =
                new SelectList(await _uow.MaterialRepository.AllAsync(), "Id", "Name", order.MaterialId);
            ViewData["ShipmentID"] =
                new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Method", order.ShipmentID);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind(
                "CustomerId,MaterialId,OrderNumber,Name,OrderDate,Deadline,TotalAmount,TotalArea,LinearMeter,Details,Status,ShipmentID,PalletNumber,BillingDate,Id")]
            Order order)
        {
            if (id != order.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.OrderRepository.Update(order);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _uow.OrderRepository.ExistsAsync(order.Id))
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

            ViewData["CustomerId"] =
                new SelectList(await _uow.CustomerRepository.AllAsync(), "Id", "Address", order.CustomerId);
            ViewData["MaterialId"] =
                new SelectList(await _uow.MaterialRepository.AllAsync(), "Id", "Name", order.MaterialId);
            ViewData["ShipmentID"] =
                new SelectList(await _uow.ShipmentRepository.AllAsync(), "Id", "Method", order.ShipmentID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var order = await _uow.OrderRepository.FindAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _uow.OrderRepository.RemoveAsync(id);
            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
