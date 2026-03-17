using ECommerce.Mobile.Services;
using ECommerce.Mobile.Views;

namespace ECommerce.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}