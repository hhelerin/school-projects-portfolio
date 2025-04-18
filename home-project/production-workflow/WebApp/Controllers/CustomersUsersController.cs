using App.DAL.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Domain;
using App.Domain.Identity;
using Base.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class CustomersUsersController : Controller
    {
        private readonly IAppUOW _uow;
        private readonly UserManager<AppUser> _userManager;

        public CustomersUsersController(IAppUOW uow, UserManager<AppUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        // GET: CustomersUsers
        public async Task<IActionResult> Index()
        {
            var customersUsers = _uow.CustomersUsersRepository.AllAsync();
            return View(await customersUsers);
        }

        // GET: CustomersUsers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _uow.CustomersUsersRepository.FindAsync(id.Value, User.GetUserId());


            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // GET: CustomersUsers/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CustomersUsersCreateEditViewModel()
            {
                CustomerSelectList = new SelectList(await _uow.CustomerRepository.AllAsync(), "Id", "Address"),
                UserSelectList = new SelectList(_userManager.Users.ToList(), "Id", "Id")

            };
            
            return View(vm);
        }

        // POST: CustomersUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomersUsersCreateEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _uow.CustomersUsersRepository.Add(vm.CustomersUsers);
                await _uow.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.CustomerSelectList = new SelectList(
                await _uow.CustomerRepository.AllAsync(),
                nameof(Customer.Id),
                nameof(Customer.Address),
                vm.CustomersUsers.CustomerId
            );

            vm.UserSelectList = new SelectList(
                _userManager.Users.ToList(),
                "Id",
                "Id",
                vm.CustomersUsers.UserId
            );

            return View(vm);
        }

        // GET: CustomersUsers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customersUsers = await _uow.CustomersUsersRepository.FindAsync(id.Value, User.GetUserId());
            if (customersUsers == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(await _uow.CustomerRepository.AllAsync(), "Id", "Address", customersUsers.CustomerId);
            ViewData["UserId"] = new SelectList(_userManager.Users.ToList(), "Id", "Id", customersUsers.UserId);
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
                    _uow.CustomersUsersRepository.Update(customersUsers);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _uow.CustomersUsersRepository.ExistsAsync(customersUsers.Id))
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
            ViewData["CustomerId"] = new SelectList(await _uow.CustomerRepository.AllAsync(), "Id", "Address", customersUsers.CustomerId);
            ViewData["UserId"] = new SelectList(_userManager.Users.ToList(), "Id", "Id", customersUsers.UserId);
            return View(customersUsers);
        }

        // GET: CustomersUsers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customersUsers =  await _uow.CustomersUsersRepository.FindAsync(id.Value, User.GetUserId());
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
            var customersUsers = await _uow.CustomersUsersRepository.FindAsync(id);
            if (customersUsers != null)
            {
                _uow.CustomersUsersRepository.Remove(customersUsers);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
