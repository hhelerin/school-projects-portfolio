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
    public class MaterialController : ControllerBase
    {
        private readonly IAppUOW _uow;

        public MaterialController(IAppUOW uow)
        {
            _uow = uow;
        }

        // GET: api/Material
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetMaterials()
        {
            return (await _uow.MaterialRepository.AllAsync(User.GetUserId())).ToList();
        }

        // GET: api/Material/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialDto>> GetMaterial(Guid id)
        {
            var material = await _uow.MaterialRepository.FindAsync(id);

            if (material == null)
            {
                return NotFound();
            }

            return material;
        }

        // PUT: api/Material/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterial(Guid id, MaterialDto material)
        {
            if (id != material.Id)
            {
                return BadRequest();
            }

            _uow.MaterialRepository.Update(material);
            
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Material
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaterialDto>> PostMaterial(MaterialDto material)
        {
            _uow.MaterialRepository.Add(material);
            await _uow.SaveChangesAsync();

            return CreatedAtAction("GetMaterial", new { id = material.Id }, material);
        }

        // DELETE: api/Material/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(Guid id)
        {
            var material = await _uow.MaterialRepository.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            _uow.MaterialRepository.Remove(material);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
