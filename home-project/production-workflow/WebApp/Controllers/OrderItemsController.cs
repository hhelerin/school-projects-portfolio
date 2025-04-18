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
    public class OrderItemsController : Controller
    {
        private readonly IAppUOW _uow;

        public OrderItemsController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            var orderItems = _uow.OrderItemRepository.AllAsync();
            return View(await orderItems);
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _uow.OrderItemRepository.FindAsync(id.Value, User.GetUserId());

            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public async Task<IActionResult> Create()
        {
            ViewData["OrderId"] = new SelectList(await _uow.OrderItemRepository.AllAsync(), "Id", "Name");
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,DetailNumber,LengthMm,WidthMm,HeightMm,Area,LinearMeter,Amount,Details,Id")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                orderItem.Id = Guid.NewGuid();
                _uow.OrderItemRepository.Add(orderItem);
                await _uow.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(await _uow.OrderItemRepository.AllAsync(), "Id", "Name", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _uow.OrderItemRepository.FindAsync(id.Value, User.GetUserId());
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(await _uow.OrderItemRepository.AllAsync(), "Id", "Name", orderItem.OrderId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OrderId,DetailNumber,LengthMm,WidthMm,HeightMm,Area,LinearMeter,Amount,Details,Id")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.OrderItemRepository.Update(orderItem);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _uow.OrderItemRepository.ExistsAsync(id))
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
            ViewData["OrderId"] = new SelectList(await _uow.OrderItemRepository.AllAsync(), "Id", "Name", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _uow.OrderItemRepository.FindAsync(id.Value, User.GetUserId());
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var orderItem = await _uow.OrderItemRepository.FindAsync(id);
            if (orderItem != null)
            {
                _uow.OrderItemRepository.Remove(orderItem);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
