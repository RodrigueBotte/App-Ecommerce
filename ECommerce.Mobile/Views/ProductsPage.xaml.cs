using ECommerce.Mobile.Services;  // Pour ProductService

namespace ECommerce.Mobile.Views;

public partial class ProductsPage : ContentPage
{
    private readonly ProductService _productService;
    private readonly AuthService _authService;

    public ProductsPage(ProductService productService, AuthService authService)  // ? Injection DI
    {
        InitializeComponent();
        _productService = productService;
        _authService = authService;
        _ = LoadProductsAsync();  // Charge produits au démarrage
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            await DisplayAlert("Debug", "Service injecté ! Port: 7251", "OK");
            var products = await _productService.GetProductsAsync();
            ProductsCollectionView.ItemsSource = products;
            await DisplayAlert("Succès", $"Produits: {products?.Count ?? 0}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("ERREUR", ex.Message, "OK");  // ? DÉTAIL !
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)  // ? Lie à XAML Clicked
    {
        await LoadProductsAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var isAdmin = await _authService.IsAdminAsync();
        AdminButton.IsVisible = isAdmin;
    }

    private async void OnAdminTapped(object sender, EventArgs e)
    {
        var adminPage = Handler.MauiContext.Services.GetRequiredService<AdminProductsPage>();
        await Navigation.PushAsync(adminPage);
    }

    // gestion du nombre d'élément affiché en fonction de la taille de l'écran
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (ProductsCollectionView.ItemsLayout is GridItemsLayout layout)
        {
            layout.Span = width switch
            {
                < 600 => 1,  // Mobile portrait
                < 1000 => 2,  // Mobile paysage / petite tablette
                _ => 3   // Tablette / Windows
            };
        }
    }

}
