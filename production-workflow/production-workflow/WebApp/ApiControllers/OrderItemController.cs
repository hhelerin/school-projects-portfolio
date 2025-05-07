using App.DAL.Contracts;
using Microsoft.AspNetCore.Mvc;
using App.DAL.DTO;
using Asp.Versioning;
using Base.Helpers;

namespace WebApp.ApiControllers
{
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IAppUOW _uow;

        public OrderItemController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItems()
        {
            return (await _uow.OrderItemRepository.AllAsync(User.GetUserId())).ToList();
        }

        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItem(Guid id)
        {
            var orderItem = await _uow.OrderItemRepository.FindAsync(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return orderItem;
        }

        // PUT: api/OrderItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(Guid id, OrderItemDto orderItem)
        {
            if (id != orderItem.Id)
            {
                return BadRequest();
            }

            _uow.OrderItemRepository.Update(orderItem);

            await _uow.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/OrderItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> PostOrderItem(OrderItemDto orderItem)
        {
            _uow.OrderItemRepository.Add(orderItem);
            await _uow.SaveChangesAsync();

            return CreatedAtAction("GetOrderItem", new { id = orderItem.Id }, orderItem);
        }

        // DELETE: api/OrderItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(Guid id)
        {
            var orderItem = await _uow.OrderItemRepository.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _uow.OrderItemRepository.Remove(orderItem);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
        
    }
}
