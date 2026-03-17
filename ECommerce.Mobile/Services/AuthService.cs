using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Mobile.Models;

namespace ECommerce.Mobile.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string TokenKey = "auth_token";
        private const string EmailKey = "user_email";

        public AuthService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7251/")
            };
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            // Serialise l'email et le password en json
            var payload = new LoginRequest { Email = email, Password = password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Envoie une requete POST à l'API pour se connecter
            var response = await _httpClient.PostAsync("api/auth/login", content);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            // Si la connexion est réussie, on récupère le token d'authentification et on le stocke de manière sécurisée
            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Désérialise la réponse JSON pour obtenir le token d'authentification
            var authtResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, options);

            if (authtResponse?.AccessToken == null)
            {
                return false;
            }

            // Stocke le token d'authentification de manière sécurisée
            await SecureStorage.SetAsync(TokenKey, authtResponse.AccessToken);

            // Stocke également l'email de l'utilisateur de manière sécurisée pour une utilisation future
            await SecureStorage.SetAsync(EmailKey, email);
            return true;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            // Serialise l'email et le password en json
            var payload = new RegisterRequest { Email = email, Password = password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);
            return response.IsSuccessStatusCode; // Retourne true si l'inscription est réussie, sinon false
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(TokenKey); // Récupère le token d'authentification stocké de manière sécurisée, null si pas connecté
        }

        public void Logout()
        {
            // Supprime le token d'authentification stocké de manière sécurisée pour se déconnecter
            SecureStorage.Remove(TokenKey);
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token); // Retourne true si un token d'authentification est présent, sinon false
        }

        public async Task<string?> GetUserEmailAsync()
        {
            return await SecureStorage.GetAsync(EmailKey); // Récupère l'email de l'utilisateur stocké de manière sécurisée, null si pas connecté
        }

        public async Task<bool> IsAdminAsync()
        {
            var token = await GetTokenAsync();

            if (string.IsNullOrEmpty(token)) return false;

            // Décode le token localement pour vérifier les rôles de l'utilisateur sans faire une requete à l'API
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Cherche un claim de type Role avec la valeur Admin
            return jwt.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        public async Task LogoutAsync()
        {
            // Supprime le token JWT du SecureStorage
            SecureStorage.Remove(TokenKey);
            // on retourne une tâche complétée pour respecter la signature async
            await Task.CompletedTask;
        }
    }
}
