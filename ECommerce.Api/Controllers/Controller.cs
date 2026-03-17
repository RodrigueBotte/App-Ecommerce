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
        private readonly IWebHostEnvironment _env; // Pour gérer les fichiers images

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
        public async Task<IActionResult> Create([FromForm] ProductDto dto) // FormForm pour gérer l'upload d'image
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = await SaveImageAsync(dto.Image) ?? string.Empty 
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
        public async Task<IActionResult> Update(int id,[FromForm] ProductDto dto) // FormForm pour gérer l'upload d'image
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

            if (dto.Image != null)
            {
                DeleteImageIfExists(product.ImageUrl);
                product.ImageUrl = await SaveImageAsync(dto.Image) ?? product.ImageUrl; // Si l'upload échoue, on garde l'ancienne image
            }

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

            DeleteImageIfExists(product.ImageUrl); // Supprime l'image du serveur si elle existe
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(); // Delete en bdd

            return NoContent();
        }

        // Méthode privées pour gérer les images

        private async Task<string?> SaveImageAsync(IFormFile? image)
        {
            // Si aucune image n'est fournie, on retourne null
            if (image == null || image.Length == 0) return null;

            // Vérification du type de fichier (seulement les images)
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(image.ContentType)) return null;

            var weebRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"); // Fallback si WebRootPath est null

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}"; // Génère un nom de fichier
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "products"); // Dossier de destination : wwwroot/uploads/products

            Directory.CreateDirectory(uploadPath); // Crée le dossier s'il n'existe pas

            var filePath = Path.Combine(uploadPath, fileName); // Chemin complet du fichier
            using var stream = new FileStream(filePath, FileMode.Create); // Crée le fichier sur le serveur
            await image.CopyToAsync(stream); // Copie le contenu de l'image dans le fichier

            return $"/uploads/products/{fileName}"; // Retourne le chemin relatif pour stocker en bdd
        }

        private void DeleteImageIfExists(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return; // Si pas d'image, il ne se passe rien

            var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar); // Reconstruit le chemin physique depuis l'url relative
            var fullPath = Path.Combine(_env.WebRootPath, relativePath); // Chemin complet du fichier

            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath); // Supprimer le fichier s'il existe
        }
    }
}
