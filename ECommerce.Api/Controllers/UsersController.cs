using System.Security.Claims;
using ECommerce.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize] // Necessite d'etre connecté et d'avoir un jwt pour y accéder
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Get api/users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            // Récupérer l'id de l'utilisateur connecté à partir du token JWT
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userID);
            if (user is null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email
            });
        }

        // Put api/users/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            // Mettre à jour les champs de l'utilisateur avec les valeurs du DTO, si elles sont fournies
            user.UserName = dto.UserName ?? user.UserName;
            user.Email = dto.Email ?? user.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Profil mis à jour avec succès" });
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            await _userManager.DeleteAsync(user);
            return Ok(new { message = "Compte supprimé avec succès}" });
        }
    }
}
