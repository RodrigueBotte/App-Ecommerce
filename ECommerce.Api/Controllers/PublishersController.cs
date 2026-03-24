using ECommerce.Api.Data;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PublishersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Publisher>>> GetAll()
        {
            return Ok(await _context.Publishers.ToListAsync());
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<Publisher>> Create([FromBody]string name)
        {
            var publisher = new Publisher { Name = name };
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new {id =  publisher.Id}, publisher);
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] string name)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher is null) return NotFound(new { message = $"Editeur {id} introuvable" });
            publisher.Name = name;
            await _context.SaveChangesAsync();
            return Ok(publisher);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher is null) return NotFound(new { message = $"Editeur {id} introuvable" });
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return Ok(publisher);
        }
    }
}
