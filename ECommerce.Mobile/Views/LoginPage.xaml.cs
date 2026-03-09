using ECommerce.Mobile.Services;

namespace ECommerce.Mobile.Views;

public partial class LoginPage : ContentPage
{
	private readonly AuthService _authService;
	private readonly RegisterPage _registerPage;
    public LoginPage(AuthService authService, RegisterPage registerPage)
	{
		InitializeComponent();
		_authService = authService;
		_registerPage = registerPage;
    }

	private async void OnLoginClicked(object sender, EventArgs e)
	{
		ErrorLabel.IsVisible = false;
		LoginButton.IsEnabled = false;
		LoginButton.Text = "Connexion en cours...";

		try
		{
			var email = EmailEntry.Text?.Trim();
			var password = PasswordEntry.Text;

            // Validation simple de l'email et du mot de passe
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
				ErrorLabel.Text = "Veillez remplir tous les champs";
				ErrorLabel.IsVisible = true;
				return;
            }

			var success = await _authService.LoginAsync(email, password);
			if (success)
			{
                // Rediriger vers la page principale de l'application
                Application.Current!.Windows[0].Page = new AppShell();
            }
			else
			{
				ErrorLabel.Text = "Email ou mot de passe incorrect";
				ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
		{
			ErrorLabel.Text = $"Erreur : {ex.Message}";
			ErrorLabel.IsVisible = true;
        }
		finally
		{
			LoginButton.IsEnabled = true;
			LoginButton.Text = "Se connecter";
        }
    }

	private async void OnRegisterTapped(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new RegisterPage(_authService));
	}
}