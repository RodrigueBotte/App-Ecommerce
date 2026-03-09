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
            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddTransient<ProductsPage>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddTransient<AdminProductsPage>();
            builder.Services.AddScoped<UserService>();

            return builder.Build();
        }
    }
}
