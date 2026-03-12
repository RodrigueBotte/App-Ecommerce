using ECommerce.Mobile.Services;
using Microsoft.Maui.Controls;


namespace ECommerce.Mobile.Views;

public partial class ProfilePage : ContentPage
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public ProfilePage(UserService userService, AuthService authService)
	{
		InitializeComponent();
        _userService = userService;
        _authService = authService;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // on vérifie si l'utilisateur est connecté
        var token = await SecureStorage.GetAsync("token");
        if (string.IsNullOrEmpty(token))
        {
            // si pas de token, on renvoie vers login
            await Shell.Current.GoToAsync("login");
            return; // on arrête le chargement de la page si pas connecté
        }
        // si connecté, on charge le profil
        await LoadProfileAsync();
    }

    private async Task LoadProfileAsync()
    {
        try
        {
            // on appelle l'api Get
            var profile = await _userService.GetProfileAsync();
            if (profile != null)
            {
                // on affiche les données dans les champs
                emailText.Text = profile.Email;
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de charger le profil", "OK");
            }
        }
        catch(Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message + "\n\n" + ex.StackTrace, "OK");
        }
    }

    private async void UpdateProfile(object sender, EventArgs e)
    {
        // on récupère le profil actuel pour pré-remplir la boîte de dialogue
        var profile = await _userService.GetProfileAsync();
        if (profile == null) return;

        // on affiche une boîte de dialogue pour saisir le nouvel email
        var newEmail = await DisplayPromptAsync(
            "Modifier le profil",
            "Nouvel email :",
            initialValue: profile.Email, // pré-rempli avec l'email actuel
            keyboard: Keyboard.Email // clavier adapté pour les emails
            );

        // si l'utilisateur annule, on ne fait rien
        if (newEmail == null) return;

        var success = await _userService.UpdateProfileAsync(profile.UserName, newEmail);

        if (success)
        {
            await DisplayAlert("Succés", "Profil mis à jour!", "OK");
            // On recharge la page pour afficher les nouvelles données
            await LoadProfileAsync();
        }
        else
        {
            await DisplayAlert("Erreur", "La mise à jour a échoué.", "OK");
        }
    }

    private async void Logout(object sender, EventArgs e)
    {
        // on demande une confirmation avant de se déconnecter
        bool confirm = await DisplayAlert(
            "Déconnexion",
            "Voulez vous vraiment vous déconnecter?",
            "Oui",
            "Non"
            );

        if (!confirm) return;

        // on appelle la méthode de déconnexion du service d'authentification
        await _authService.LogoutAsync();

        // on redirige vers la page de connexion
        await Shell.Current.GoToAsync("//main");
    }
}