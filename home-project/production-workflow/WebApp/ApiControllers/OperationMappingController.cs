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
    public class OperationMappingController : ControllerBase
    {
        private readonly IAppUOW _uow;

        public OperationMappingController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: api/OperationMapping
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationMappingDto>>> GetOperationMappings()
        {
            return (await _uow.OperationMappingRepository.AllAsync(User.GetUserId())).ToList();;
        }

        // GET: api/OperationMapping/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperationMappingDto>> GetOperationMapping(Guid id)
        {
            var operationMapping = await _uow.OperationMappingRepository.FindAsync(id);

            if (operationMapping == null)
            {
                return NotFound();
            }

            return operationMapping;
        }

        // PUT: api/OperationMapping/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperationMapping(Guid id, OperationMappingDto operationMapping)
        {
            if (id != operationMapping.Id)
            {
                return BadRequest();
            }

            _uow.OperationMappingRepository.Update(operationMapping);
            
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/OperationMapping
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OperationMappingDto>> PostOperationMapping(OperationMappingDto operationMapping)
        {
            _uow.OperationMappingRepository.Add(operationMapping);
            await _uow.SaveChangesAsync();

            return CreatedAtAction("GetOperationMapping", new { id = operationMapping.Id }, operationMapping);
        }

        // DELETE: api/OperationMapping/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperationMapping(Guid id)
        {
            var operationMapping = await _uow.OperationMappingRepository.FindAsync(id);
            if (operationMapping == null)
            {
                return NotFound();
            }

            _uow.OperationMappingRepository.Remove(operationMapping);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
