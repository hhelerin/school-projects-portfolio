using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomersUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CustomersUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomersUsers>>> GetCustomersUsers()
        {
            return await _context.CustomersUsers.ToListAsync();
        }

        // GET: api/CustomersUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomersUsers>> GetCustomersUsers(Guid id)
        {
            var customersUsers = await _context.CustomersUsers.FindAsync(id);

            if (customersUsers == null)
            {
                return NotFound();
            }

            return customersUsers;
        }

        // PUT: api/CustomersUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomersUsers(Guid id, CustomersUsers customersUsers)
        {
            if (id != customersUsers.Id)
            {
                return BadRequest();
            }

            _context.Entry(customersUsers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomersUsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CustomersUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomersUsers>> PostCustomersUsers(CustomersUsers customersUsers)
        {
            _context.CustomersUsers.Add(customersUsers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomersUsers", new { id = customersUsers.Id }, customersUsers);
        }

        // DELETE: api/CustomersUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomersUsers(Guid id)
        {
            var customersUsers = await _context.CustomersUsers.FindAsync(id);
            if (customersUsers == null)
            {
                return NotFound();
            }

            _context.CustomersUsers.Remove(customersUsers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomersUsersExists(Guid id)
        {
            return _context.CustomersUsers.Any(e => e.Id == id);
        }
    }
}
