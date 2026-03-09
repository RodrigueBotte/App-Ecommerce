using ECommerce.Mobile.Services;

namespace ECommerce.Mobile.Views;

public partial class RegisterPage : ContentPage
{
	private readonly AuthService _authService;
	public RegisterPage(AuthService authService)
	{
		InitializeComponent();
		_authService = authService;
	}

	private async void OnRegisterClicked(object sender, EventArgs e)
	{
		MessageLabel.IsVisible = false;
		RegisterButton.IsEnabled = false;
		RegisterButton.Text = "Création...";

		try
		{
			var email = EmailEntry.Text?.Trim();
			var password = PasswordEntry.Text;
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				MessageLabel.Text = "Veuillez remplir tous les champs.";
				MessageLabel.IsVisible = true;
				MessageLabel.TextColor = Colors.Red;
				return;
			}
			var success = await _authService.RegisterAsync(email, password);

			if (success)
			{
				MessageLabel.Text = "Compte créé! Redirection...";
				MessageLabel.IsVisible = true;
				MessageLabel.TextColor = Colors.Green;
				await Task.Delay(1500);
                await Navigation.PopAsync();
            }
			else
			{
				MessageLabel.Text = "Erreur : email déjà utilisé ou mot de passe trop faible.";
				MessageLabel.TextColor = Colors.Red;
				MessageLabel.IsVisible = true;
			}
		}
		catch (Exception ex)
		{
			MessageLabel.Text = $"Erreur : {ex.Message}";
			MessageLabel.TextColor = Colors.Red;
			MessageLabel.IsVisible = true;
		}
		finally
		{
			RegisterButton.IsEnabled = true;
			RegisterButton.Text = "Créer mon compte";
		}
	}
	private async void OnLoginTapped(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
    }
}