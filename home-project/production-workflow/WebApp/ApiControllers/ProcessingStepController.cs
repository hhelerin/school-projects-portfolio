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
    public class ProcessingStepController : ControllerBase
    {
        private readonly IAppUOW _uow;

        public ProcessingStepController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: api/ProcessingStep
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcessingStepDto>>> GetProcessingSteps()
        {
            return (await _uow.ProcessingStepRepository.AllAsync(User.GetUserId())).ToList();;
        }

        // GET: api/ProcessingStep/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessingStepDto>> GetProcessingStep(Guid id)
        {
            var processingStep = await _uow.ProcessingStepRepository.FindAsync(id);

            if (processingStep == null)
            {
                return NotFound();
            }

            return processingStep;
        }

        // PUT: api/ProcessingStep/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcessingStep(Guid id, ProcessingStepDto processingStep)
        {
            if (id != processingStep.Id)
            {
                return BadRequest();
            }

            _uow.ProcessingStepRepository.Update(processingStep);
            
            await _uow.SaveChangesAsync();
            
            return NoContent();
        }

        // POST: api/ProcessingStep
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProcessingStepDto>> PostProcessingStep(ProcessingStepDto processingStep)
        {
            _uow.ProcessingStepRepository.Add(processingStep);
            await _uow.SaveChangesAsync();

            return CreatedAtAction("GetProcessingStep", new { id = processingStep.Id }, processingStep);
        }

        // DELETE: api/ProcessingStep/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcessingStep(Guid id)
        {
            var processingStep = await _uow.ProcessingStepRepository.FindAsync(id);
            if (processingStep == null)
            {
                return NotFound();
            }

            _uow.ProcessingStepRepository.Remove(processingStep);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
