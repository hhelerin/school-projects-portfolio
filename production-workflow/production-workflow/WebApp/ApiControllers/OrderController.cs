using App.DAL.Contracts;
using App.DAL.DTO;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Base.Helpers;
using Microsoft.EntityFrameworkCore;

namespace WebApp.ApiControllers
{
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAppUOW _uow;

        public OrderController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            return (await _uow.OrderRepository.AllAsync(User.GetUserId())).ToList();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var order = await _uow.OrderRepository.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Order/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(Guid id, OrderDto order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _uow.OrderRepository.Update(order);

            await _uow.SaveChangesAsync();
            
            return NoContent();
        }

        // POST: api/Order
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDto>> PostOrder(OrderDto order)
        {
            _uow.OrderRepository.Add(order);
            await _uow.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _uow.OrderRepository.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _uow.OrderRepository.Remove(order);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
