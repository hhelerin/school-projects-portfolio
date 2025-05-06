using App.DAL.Contracts;
using App.DAL.DTO;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Base.Helpers;

namespace WebApp.ApiControllers
{
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IAppUOW _uow;

        public ShipmentController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: api/Shipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetShipments()
        {
            return (await _uow.ShipmentRepository.AllAsync(User.GetUserId())).ToList();;
        }

        // GET: api/Shipment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentDto>> GetShipment(Guid id)
        {
            var shipment = await _uow.ShipmentRepository.FindAsync(id);

            if (shipment == null)
            {
                return NotFound();
            }

            return shipment;
        }

        // PUT: api/Shipment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShipment(Guid id, ShipmentDto shipment)
        {
            if (id != shipment.Id)
            {
                return BadRequest();
            }

            _uow.ShipmentRepository.Update(shipment);
            
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Shipment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShipmentDto>> PostShipment(ShipmentDto shipment)
        {
            _uow.ShipmentRepository.Add(shipment);
            await _uow.SaveChangesAsync();

            return CreatedAtAction("GetShipment", new { id = shipment.Id }, shipment);
        }

        // DELETE: api/Shipment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipment(Guid id)
        {
            var shipment = await _uow.ShipmentRepository.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            _uow.ShipmentRepository.Remove(shipment);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
