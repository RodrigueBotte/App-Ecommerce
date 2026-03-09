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
        var email = await _authService.GetUserEmailAsync();
        WelcomeLabel.Text = $"Welcome, {email}!";
    }
}
