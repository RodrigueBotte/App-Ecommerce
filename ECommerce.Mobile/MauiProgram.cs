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
            // AddSingleton sert à créer une instance unique de ProductService qui sera partagée dans toute l'application
            // AddTransient crée une nouvelle instance de ProductsPage chaque fois qu'elle est demandée
            // AddScoped crée une instance de UserService qui est partagée dans le contexte d'une portée spécifique, comme une session utilisateur ou une requête
            // Cela permet de gérer efficacement les ressources et d'assurer que les pages sont réinitialisées à chaque utilisation, tandis que les services partagent des données et des états communs.
            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddTransient<ProductsPage>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AdminProductsPage>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<ProfilePage>();

            return builder.Build();
        }
    }
}
        