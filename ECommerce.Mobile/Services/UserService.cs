using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.Mobile.Models;

namespace ECommerce.Mobile.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public UserService(AuthService authService)
        {
            _authService = authService;
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7251/"),
            };
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await _authService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<UserProfile?> GetProfileAsync()
        {
            await SetAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<UserProfile>("api/users/me");
        }

        public async Task<bool> UpdateProfileAsync(string userName, string email)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync("api/users/me",
                new { UserName = userName, Email = email });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAccount()
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync("api/users/me");
            return response.IsSuccessStatusCode;
        }
    }
}
