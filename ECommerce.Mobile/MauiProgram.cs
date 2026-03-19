using ECommerce.Mobile.Services;
using ECommerce.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace ECommerce.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // AddSingleton sert à créer une instance unique de de la page en question qui sera partagée dans toute l'application
            // AddTransient crée une nouvelle instance de la page en question à chaque fois qu'elle est demandée
            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddTransient<ProductsPage>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AdminProductsPage>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<ProfilePage>();
            builder.Services.AddSingleton<ProductDetailPage>();

            return builder.Build();
        }
    }
}
        