using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationMappingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OperationMappingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OperationMapping
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationMapping>>> GetOperationMappings()
        {
            return await _context.OperationMappings.ToListAsync();
        }

        // GET: api/OperationMapping/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperationMapping>> GetOperationMapping(Guid id)
        {
            var operationMapping = await _context.OperationMappings.FindAsync(id);

            if (operationMapping == null)
            {
                return NotFound();
            }

            return operationMapping;
        }

        // PUT: api/OperationMapping/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperationMapping(Guid id, OperationMapping operationMapping)
        {
            if (id != operationMapping.Id)
            {
                return BadRequest();
            }

            _context.Entry(operationMapping).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperationMappingExists(id))
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

        // POST: api/OperationMapping
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OperationMapping>> PostOperationMapping(OperationMapping operationMapping)
        {
            _context.OperationMappings.Add(operationMapping);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOperationMapping", new { id = operationMapping.Id }, operationMapping);
        }

        // DELETE: api/OperationMapping/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperationMapping(Guid id)
        {
            var operationMapping = await _context.OperationMappings.FindAsync(id);
            if (operationMapping == null)
            {
                return NotFound();
            }

            _context.OperationMappings.Remove(operationMapping);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OperationMappingExists(Guid id)
        {
            return _context.OperationMappings.Any(e => e.Id == id);
        }
    }
}
