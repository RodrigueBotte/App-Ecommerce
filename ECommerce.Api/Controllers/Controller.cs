using ECommerce.Api.Data;
using ECommerce.Api.DTOs;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        // Public: tout le monde peut voir les produits sans être connecté
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // GET /api/products/{id}
        // Public : récupère un produit par son id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound(new { message = $"Produit {id} introuvable" });
            }

            return Ok(product);
        }

        // Post /api/products
        // Protégé: réservé au rôle Admin
        // -> 401 si pas de token / 403 si token sans rôle Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl ?? string.Empty
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(); //Sauvegarde en bdd -> génère l'id

            //201 Created avec l'url du nouveau produit dans le header Location
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT /api/products/{id}
        // Protégé: réservé au role Admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound(new { message = $"Produit {id} introuvable" });
            }

            // on écrase les champs avec les nouvelles valeurs du DTO
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync(); // Sauvegarde en bdd

            // 204 No Content : succès mais pas de corps dans la réponse (standard REST)
            return NoContent();
        }

        // DELETE /api/products/{id}
        // Protégé: réservé au role Admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound(new { message = $"¨Produit {id} introuvable" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(); // Delete en bdd

            return NoContent();
        }
    }
}
