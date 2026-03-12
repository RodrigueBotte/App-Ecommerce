using ECommerce.Mobile.Views;

namespace ECommerce.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
            Routing.RegisterRoute("login", typeof(LoginPage));
        }
    }
}
