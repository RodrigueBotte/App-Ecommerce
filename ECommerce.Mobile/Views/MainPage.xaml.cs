using ECommerce.Mobile.Services;

namespace ECommerce.Mobile.Views;

public partial class MainPage : ContentPage
{
    private readonly AuthService _authService;
    public MainPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await SecureStorage.GetAsync("token");
        if (string.IsNullOrEmpty(token))
        {
            // si pas de token, on renvoie vers login
            WelcomeLabel.Text = $"Bienvenue sur l'app!";
            return; // on arrête le chargement de la page si pas connecté
        }
        var email = await _authService.GetUserEmailAsync();
        WelcomeLabel.Text = $"Welcome, {email}!";
    }
}
