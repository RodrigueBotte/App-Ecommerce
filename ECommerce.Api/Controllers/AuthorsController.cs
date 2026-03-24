using ECommerce.Api.Data;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthorsController(AppDbContext context)
        {
            _context = context;
        }

        // Get /api/authors
        [HttpGet]
        public async Task<ActionResult<List<Author>>> GetAll()
        {
            return Ok(await  _context.Authors.ToListAsync());
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<Author>> Create([FromBody] string name)
        {
            var author = new Author { Name = name };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = author.Id }, author);
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]string name)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound(new { message = $"Auteur {id} introuvable" });

            author.Name = name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound(new { message = $"Auteur {id} introuvable" });

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
