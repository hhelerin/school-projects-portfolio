using App.DAL.Contracts;
using App.DAL.DTO;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.ApiControllers
{
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomersUsersController : ControllerBase
    {
        private readonly IAppUOW _uow;

        public CustomersUsersController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: api/CustomersUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomersUsersDto>>> GetCustomersUsers()
        {
            return (await _uow.CustomersUsersRepository.AllAsync(User.GetUserId())).ToList();
        }

        // GET: api/CustomersUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomersUsersDto>> GetCustomersUsers(Guid id)
        {
            var customersUsers = await _uow.CustomersUsersRepository.FindAsync(id);

            if (customersUsers == null)
            {
                return NotFound();
            }

            return customersUsers;
        }

        // PUT: api/CustomersUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomersUsers(Guid id, CustomersUsersDto customersUsers)
        {
            if (id != customersUsers.Id)
            {
                return BadRequest();
            }

            _uow.CustomersUsersRepository.Update(customersUsers);

            await _uow.SaveChangesAsync();
            
            return NoContent();
        }

        // POST: api/CustomersUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomersUsersDto>> PostCustomersUsers(CustomersUsersDto customersUsers)
        {
            _uow.CustomersUsersRepository.Add(customersUsers);
            await _uow.SaveChangesAsync();

            return CreatedAtAction("GetCustomersUsers", new { id = customersUsers.Id }, customersUsers);
        }

        // DELETE: api/CustomersUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomersUsers(Guid id)
        {
            var customersUsers = await _uow.CustomersUsersRepository.FindAsync(id);
            if (customersUsers == null)
            {
                return NotFound();
            }

            _uow.CustomersUsersRepository.Remove(customersUsers);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
