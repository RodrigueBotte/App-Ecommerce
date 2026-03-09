using System.Text.Json;
using ECommerce.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ECommerce.Mobile.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ProductService(AuthService authService)
    {
        // Injection de AuthService pour les futurs besoins d'authentification (ex: token)
        _authService = authService;

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
        };

        _httpClient = new HttpClient(handler);
        // URL de base de l'API backend 
        _httpClient.BaseAddress = new Uri("https://localhost:7251/");
    }

    // Ajoute le token JWT dans le header avant chaque requete protégée
    public async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    // Get tous les produits depuis le backend
    public async Task<List<Product>> GetProductsAsync()
    {
        string json = "";
        try
        {
            json = await _httpClient.GetStringAsync("api/products");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // name → Name
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // camelCase backend ?
            };

            List<Product> products = JsonSerializer.Deserialize<List<Product>>(json, options) ?? [];
            return products;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"JSON: {json}");  // ← Vois JSON brut !
            System.Diagnostics.Debug.WriteLine($"Erreur: {ex}");
            return [];
        }
    }

    // Post un nouveau produit avec Admin uniquement
    public async Task<bool> CreateProductAsync(ProductDto dto)
    {
        try
        {
            await SetAuthHeaderAsync(); // ← Assure que le token est présent
            var response = await _httpClient.PostAsJsonAsync("api/products", dto);

            System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Body: {body}");

            return response.IsSuccessStatusCode;
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            return false;
        }
        
    }

    // Put pour update un produit existant avec Admin uniquement
    public async Task<bool> UpdateProductAsync(int id, ProductDto dto)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    // Delete un produit avec Admin uniquement
    public async Task<bool> DeleteProductAsync(int id)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }
}
