using ECommerce.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        // Injection de UserManager et SignInManager pour gérer les utilisateurs et l'authentification
        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "Utilisateur créé" });
            }
            return BadRequest(result.Errors);
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if ( user == null)
            {
                return Unauthorized();
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }
            var token = await GenerateJwtToken(user);
            return Ok(new { accessToken = token, expiresAt = DateTime.UtcNow.AddHours(1) });
        }

        // Méthode pour générer un token JWT pour l'utilisateur authentifié
        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var jwtKey = "SuperSecretKeyDevOnly1234567890123456";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var issuer = "ECommerce.Api";

            // Récupération des rôles de l'utilisateur pour les inclure dans les claims du token
            var roles = await _userManager.GetRolesAsync(user);

            // Création des claims pour le token, incluant l'id de l'utilisateur, son email et un identifiant unique pour le token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Ajout des rôles de l'utilisateur en tant que claims de type Role
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Création des credentials de signature pour le token en utilisant la clé secrète et l'algorithme HMAC SHA256
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Création du token JWT avec les informations d'issuer, audience, claims, date d'expiration et credentials de signature
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
