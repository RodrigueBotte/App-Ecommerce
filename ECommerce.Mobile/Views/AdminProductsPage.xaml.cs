using ECommerce.Mobile.Models;
using ECommerce.Mobile.Services;

namespace ECommerce.Mobile.Views;

public partial class AdminProductsPage : ContentPage
{
	private readonly ProductService _productService;
	private FileResult? _pickedImage; // Stocke l'image sélectionnée temporairement

	//sotckage des éléments sélectionnable
	private List<int> _selectedAuthorIds = [];
	private List<int> _selectedCategoryIds = [];
	private List<int> _selectedThemeIds = [];

    private int? _editingProductId = null; // null = mode ajout, sinon mode édition

    public AdminProductsPage(ProductService productService)
	{
		InitializeComponent();
		_productService = productService;
    }

	//Chargement de la page -> on charge la liste des produits
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadProductAsync();
		await LoadSelectorAsync();
	}

	// ---------------------Chargement des éléments--------------------------
	// Récupère les produits depuis l'api et les affiche dans la CollectionView
	private async Task LoadProductAsync()
	{
		var products = await _productService.GetProductsAsync();
		ProductsCollection.ItemsSource = products;
	}

	private async Task LoadSelectorAsync()
	{
		PublisherPicker.ItemsSource = await _productService.GetPublishersAsync(); 
		AuthorsCollection.ItemsSource = await _productService.GetAuthorsAsync();
		CategoriesCollection.ItemsSource = await _productService.GetCategoriesAsync();
		ThemesCollection.ItemsSource = await _productService.GetThemesAsync();
	}

	// ----------------Sélections multiple----------------------------
	private void OnAuthorsSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_selectedAuthorIds = e.CurrentSelection.OfType<Author>().Select(a=> a.Id).ToList();
	}
    private void OnCategoriesSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedCategoryIds = e.CurrentSelection.OfType<Category>().Select(c => c.Id).ToList();
    }

    private void OnThemesSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedThemeIds = e.CurrentSelection.OfType<Theme>().Select(t => t.Id).ToList();
    }

    // -------------------Image---------------------------------------
    private async void OnPickImageClicked(object sender, EventArgs e)
	{
		// On ajoute l'affichage de tous les types de photos à la sélection des photos
        var options = new PickOptions
        {
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI,   new[] { ".jpg", ".jpeg", ".png", ".webp" } },
            { DevicePlatform.Android, new[] { "image/jpeg", "image/png", "image/webp" } },
            { DevicePlatform.iOS,     new[] { "public.image" } },
        })
        };

        var result = await FilePicker.PickAsync(options);
        if (result is null) return;
        _pickedImage = result;

		//prévisualisation
		ImagePreview.Source = ImageSource.FromFile(result.FullPath);
		ImagePreview.IsVisible = true;
	}

	//---------------------------Ajouter----------------------------------
	// Bounton "ajouter un produit"
	private async void OnAddProductClicked(object sender, EventArgs e)
	{
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(PriceEntry.Text))
        {
            ShowStatus("Nom et prix obligatoires.", isError: true);
            return;
        }

        if (PublisherPicker.SelectedItem is not Publisher SelectedPublisher)
        {
            ShowStatus("Veuillez sélectionner un éditeur.", isError: true);
            return;
        }

        var dto = new ProductDto
		{
			Name = NameEntry.Text.Trim(),
			Description = DescriptionEntry.Text?.Trim() ?? string.Empty,
            // Gère , ou . pour le prix
            Price = decimal.TryParse(
				PriceEntry.Text?.Replace(',', '.'),
				System.Globalization.NumberStyles.Number,
				System.Globalization.CultureInfo.InvariantCulture,
				out var price) ? price : 0,
            Stock = int.TryParse(StockEntry.Text, out var stock) ? stock : 0,
			MinPlayers = int.TryParse(MinPlayersEntry.Text, out var minP) ? minP : 1,
			MaxPlayers = int.TryParse(MaxPlayersEntry.Text, out var maxP) ? maxP : 1,
			MinAge = int.TryParse(MinAgeEntry.Text, out var minAge) ? minAge : 0,
			GameDuration = int.TryParse(GameDurationEntry.Text, out var gameDuration) ? gameDuration : 0,
			PublisherId = SelectedPublisher.Id,
			AuthorIds = _selectedAuthorIds,
			CategoryIds = _selectedCategoryIds,
			ThemeIds = _selectedThemeIds
		};

        var success = await _productService.CreateProductAsync(dto, _pickedImage); // On passe le dto et l'image sélectionnée
        if (success)
        {
            ShowStatus("Produit ajouté !", isError: false);
			ClearForm();
			await LoadProductAsync();
        }
		else
		{
            ShowStatus("Erreur lors de l'ajout.", isError: true);
        }
    }

	//-------------------Modifier------------------------------------
	private async void OnUpdateProductClicked(object sender, EventArgs e)
	{
		if (sender is not Button btn || btn.CommandParameter is not Product product) return;

		_editingProductId = product.Id;

		NameEntry.Text = product.Name;
		DescriptionEntry.Text = product.Description;
		PriceEntry.Text = product.Price.ToString(System.Globalization.CultureInfo.InvariantCulture);
		StockEntry.Text = product.Stock.ToString();
		MinPlayersEntry.Text = product.MinPlayers.ToString();
		MaxPlayersEntry.Text = product.MaxPlayers.ToString();
		MinAgeEntry.Text = product.MinAge.ToString();
		GameDurationEntry.Text = product.GameDuration.ToString();

		// Il faut sélectionner le bon publisher dans le picker
		var publishers = PublisherPicker.ItemsSource as List<Publisher>;
		PublisherPicker.SelectedItem = publishers?.FirstOrDefault(p => p.Id == product.PublisherId);

		//Préremplir les selections multiples
		var authors = AuthorsCollection.ItemsSource as List<Author>;
		AuthorsCollection.SelectedItems?.Clear();
        foreach (var author in product.Authors)
        {
            var match = authors?.FirstOrDefault(a=> a.Id == author.Id);
			if (match != null) AuthorsCollection.SelectedItems?.Add(match);
        }

        var categories = CategoriesCollection.ItemsSource as List<Category>;
        CategoriesCollection.SelectedItems?.Clear();
        foreach (var category in product.Categories)
        {
            var match = categories?.FirstOrDefault(c => c.Id == category.Id);
            if (match != null) CategoriesCollection.SelectedItems?.Add(match);
        }

        var themes = ThemesCollection.ItemsSource as List<Theme>;
        ThemesCollection.SelectedItems?.Clear();
        foreach (var theme in product.Themes)
        {
            var match = themes?.FirstOrDefault(t => t.Id == theme.Id);
            if (match != null) ThemesCollection.SelectedItems?.Add(match);
        }

        // On change le bouton Ajouter en Mettre à jour
		AddButton.Text = "Mettre à jour";
        AddButton.BackgroundColor = Color.FromArgb("#FF9800");
		// on retire l'ancien comportement
        AddButton.Clicked -= OnAddProductClicked;
		// on remplace par le nouveau comportement du bouton
		AddButton.Clicked += async (s, args) =>
		{
			var dto = new ProductDto
			{
				Name = NameEntry.Text.Trim(),
				Description = DescriptionEntry.Text?.Trim() ?? string.Empty,
				Price = decimal.TryParse(
					PriceEntry.Text?.Replace(',', '.'),
					System.Globalization.NumberStyles.Number,
					System.Globalization.CultureInfo.InvariantCulture,
					out var p) ? p : product.Price,
				Stock = int.TryParse(StockEntry.Text, out var s2) ? s2 : product.Stock,
				MinPlayers = int.TryParse(MinPlayersEntry.Text, out var minP) ? minP : product.MinPlayers,
				MaxPlayers = int.TryParse(MaxPlayersEntry.Text, out var maxP) ? maxP : product.MaxPlayers,
				MinAge = int.TryParse(MinAgeEntry.Text, out var minA) ? minA : product.MinAge,
				GameDuration = int.TryParse(GameDurationEntry.Text, out var dur) ? dur : product.GameDuration,
				PublisherId = (PublisherPicker.SelectedItem as Publisher)?.Id ?? product.PublisherId,
				AuthorIds = _selectedAuthorIds,
				CategoryIds = _selectedCategoryIds,
				ThemeIds = _selectedThemeIds
			};
			var success = await _productService.UpdateProductAsync(product.Id, dto, _pickedImage);
            if (success)
            {
                ShowStatus("Produit modifié !", isError: false);
				ClearForm();
				await LoadProductAsync();

				AddButton.Text = "Ajouter le produit";
                AddButton.BackgroundColor = Color.FromArgb("#4CAF50");
                AddButton.Clicked -= null;
                AddButton.Clicked += OnAddProductClicked;
            }
			else
			{
				ShowStatus("Erreur lors de la modification", isError: true);
			}
        };
		// scroll vers le haut de la page pour voir le formulaire
		await ((ScrollView)Content).ScrollToAsync(0, 0, true);
    }

	//------------------Supprimer--------------------------
	// Bouton "supprimer le produit"
	private async void OnDeleteProductClicked(object sender, EventArgs e)
	{
        if (sender is Button btn && btn.CommandParameter is int productId)
        {
			bool confirm = await DisplayAlert(
					"Confirmation",
					"Supprimr ce produit?",
					"Oui", "Non");

            if (!confirm)
			{
				return;
            }
			var success = await _productService.DeleteProductAsync(productId);
		    if (success)
            {
				await LoadProductAsync();
			}
			else
			{
                ShowStatus("Erreur lors de la suppression.", isError: true);
            }
        }
    }

	// Affiche le message de status
	private void ShowStatus(string message, bool isError)
	{
		StatusLabel.Text = message;
		StatusLabel.TextColor = isError ? Colors.Red : Color.FromArgb("#4CAF50");
		StatusLabel.IsVisible = true;
    }

	// Vide les champs du formulaire
	private void ClearForm()
	{
        NameEntry.Text = string.Empty;
        DescriptionEntry.Text = string.Empty;
        PriceEntry.Text = string.Empty;
        StockEntry.Text = string.Empty;
        MinPlayersEntry.Text = string.Empty;
        MaxPlayersEntry.Text = string.Empty;
        MinAgeEntry.Text = string.Empty;
        GameDurationEntry.Text = string.Empty;
        PublisherPicker.SelectedItem = null;
        AuthorsCollection.SelectedItems?.Clear();
        CategoriesCollection.SelectedItems?.Clear();
        ThemesCollection.SelectedItems?.Clear();
        _selectedAuthorIds = [];
        _selectedCategoryIds = [];
        _selectedThemeIds = [];
        ImagePreview.IsVisible = false;
        ImagePreview.Source = null;
        _pickedImage = null;
    }
}