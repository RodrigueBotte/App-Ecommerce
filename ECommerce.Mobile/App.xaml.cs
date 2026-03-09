using ECommerce.Mobile.Services;
using ECommerce.Mobile.Views;

namespace ECommerce.Mobile
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        public App(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Résout LoginPage via DI correctement
            var loginPage = Handler.MauiContext!.Services.GetRequiredService<LoginPage>();
            return new Window(new NavigationPage(loginPage));
        }
    }
}