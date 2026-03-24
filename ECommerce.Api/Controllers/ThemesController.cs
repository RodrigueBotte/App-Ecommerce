using ECommerce.Api.Data;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThemesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThemesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Theme>>> GetAll()
        {
            return Ok(await  _context.Themes.ToListAsync());
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<Theme>> Create([FromBody] string name)
        {
            var theme = new Theme { Name = name };
            _context.Themes.Add(theme);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = theme.Id }, theme);
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] string name)
        {
            var theme = await _context.Themes.FindAsync(id);
            if (theme is null) return NotFound(new { message = $"Theme {id} introuvable" });
            theme.Name = name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var theme = await _context.Themes.FindAsync(id);
            if (theme is null) return NotFound(new { message = $"Theme {id} introuvable" });
            _context.Themes.Remove(theme);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
