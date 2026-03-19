using ECommerce.Mobile.Services;

namespace ECommerce.Mobile.Views;

public partial class MainPage : ContentPage
{
    private readonly ProductService _productService;
    public MainPage(ProductService productService)
    {
        InitializeComponent();
        _productService = productService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadLatestProducts();
    }

    private async Task LoadLatestProducts()
    {
        try
        {
            var products = await _productService.GetProductsAsync();
            // Récupèrer seulement les trois derniers produits
            var latest = products.OrderByDescending(p => p.Id).Take(3).ToList();

            LatestProductsCollection.ItemsSource = latest;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les produits: {ex.Message}", "Ok");
        }
    }

    private async void OnSeeAllClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("productPage");
    }
}
