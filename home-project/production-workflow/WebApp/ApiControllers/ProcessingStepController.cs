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
    public class ProcessingStepController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProcessingStepController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProcessingStep
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcessingStep>>> GetProcessingSteps()
        {
            return await _context.ProcessingSteps.ToListAsync();
        }

        // GET: api/ProcessingStep/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessingStep>> GetProcessingStep(Guid id)
        {
            var processingStep = await _context.ProcessingSteps.FindAsync(id);

            if (processingStep == null)
            {
                return NotFound();
            }

            return processingStep;
        }

        // PUT: api/ProcessingStep/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcessingStep(Guid id, ProcessingStep processingStep)
        {
            if (id != processingStep.Id)
            {
                return BadRequest();
            }

            _context.Entry(processingStep).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcessingStepExists(id))
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

        // POST: api/ProcessingStep
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProcessingStep>> PostProcessingStep(ProcessingStep processingStep)
        {
            _context.ProcessingSteps.Add(processingStep);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProcessingStep", new { id = processingStep.Id }, processingStep);
        }

        // DELETE: api/ProcessingStep/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcessingStep(Guid id)
        {
            var processingStep = await _context.ProcessingSteps.FindAsync(id);
            if (processingStep == null)
            {
                return NotFound();
            }

            _context.ProcessingSteps.Remove(processingStep);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProcessingStepExists(Guid id)
        {
            return _context.ProcessingSteps.Any(e => e.Id == id);
        }
    }
}
