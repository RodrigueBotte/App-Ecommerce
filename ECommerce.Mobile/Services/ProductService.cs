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
        var response = await _httpClient.GetAsync("api/products");
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadFromJsonAsync<List<Product>>() ?? new List<Product>();

        foreach (var product in products)
        {
            if (!string.IsNullOrEmpty(product.ImageUrl)) product.ImageUrl = $"https://localhost:7251{product.ImageUrl}";
        }
        return products;
    }

    // Post un nouveau produit avec Admin uniquement
    public async Task<bool> CreateProductAsync(ProductDto dto, FileResult? image = null)
    {
        try
        {
            await SetAuthHeaderAsync();

            using var content = new MultipartFormDataContent(); // Permet d'envoyer à la fois des champs texte et un fichier (image)

            // Champs texte
            content.Add(new StringContent(dto.Name), "name");
            content.Add(new StringContent(dto.Description), "description");
            content.Add(new StringContent(dto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "price"); // InvariantCulture pour éviter les problèmes de format (ex: 19.99 vs 19,99)
            content.Add(new StringContent(dto.Stock.ToString()), "stock");

            await Shell.Current.DisplayAlert("Debug Service",
                $"Prix dans le dto: {dto.Price}\nPrix envoyé: {dto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
                "OK");

            // Champs image(optionnel)
            if (image != null)
            {
                var stream = await image.OpenReadAsync(); // Ouvre le flux de l'image
                var imageContent = new StreamContent(stream); // Crée un StreamContent pour l'image
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType ?? "image/jpeg"); // Fallback à image/jpeg si ContentType est null
                content.Add(imageContent, "image", image.FileName); // "image" est le nom du champs attendu par le backend
            }

            var response = await _httpClient.PostAsync("api/products", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status: {(int)response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Body: {error}");
                await Shell.Current.DisplayAlert($"Erreur {(int)response.StatusCode}", error, "OK");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }

    // Put pour update un produit existant avec Admin uniquement
    public async Task<bool> UpdateProductAsync(int id, ProductDto dto, FileResult? image = null)
    {
        try
        {
            await SetAuthHeaderAsync();

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Name), "name");
            content.Add(new StringContent(dto.Description), "description");
            content.Add(new StringContent(dto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "price");
            content.Add(new StringContent(dto.Stock.ToString()), "stock");

            if (image != null)
            {
                var stream = await image.OpenReadAsync();
                var imageContent = new StreamContent(stream);
                imageContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType ?? "image/jpeg");

                content.Add(imageContent, "image", image.FileName);
            }

            var response = await _httpClient.PutAsync($"api/products/{id}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }

    // Delete un produit avec Admin uniquement
    public async Task<bool> DeleteProductAsync(int id)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }
}
