using ECommerce.Mobile.Models;
using ECommerce.Mobile.Services;

namespace ECommerce.Mobile.Views;

public partial class AdminProductsPage : ContentPage
{
	private readonly ProductService _productService;
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
	}

	// Récupère les produits depuis l'api et les affiche dans la CollectionView
	private async Task LoadProductAsync()
	{
		var products = await _productService.GetProductsAsync();
		ProductsCollection.ItemsSource = products;
	}

	// Bounton "ajouter un produit"
	private async void OnAddProductClicked(object sender, EventArgs e)
	{
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(PriceEntry.Text))
        {
            ShowStatus("Nom et prix obligatoires.", isError: true);
            return;
        }

		var dto = new ProductDto
		{
			Name = NameEntry.Text.Trim(),
			Description = DescriptionEntry.Text?.Trim() ?? string.Empty,
			Price = decimal.TryParse(PriceEntry.Text, out var price) ? price : 0,
			Stock = int.TryParse(StockEntry.Text, out var stock) ? stock : 0,
			ImageUrl = ImageUrlEntry.Text?.Trim()
		};

		var success = await _productService.CreateProductAsync(dto);
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

	private async void OnUpdateProductClicked(object sender, EventArgs e)
	{
        if (sender is Button btn && btn.CommandParameter is int productId)
        {
			// ItemsSource permet d'accéder aux données actuelles
			// Et préremplir les champs avec les valeurs existantes
			var products = ProductsCollection.ItemsSource as List<Product>;
			var product = products?.FirstOrDefault(p => p.Id == productId);

            if (product is null)
            {
                ShowStatus("Produit introuvable.", isError: true);
                return;
            }

			var name = await DisplayPromptAsync(
				"Modifier le nom du produit",
				"Nom : ",
				initialValue: product.Name);

			if (name is null) return;

			var description = await DisplayPromptAsync(
				"Modifier la description",
				"Description : ",
				initialValue: product.Description);

            if (description is null) return;

            var priceStr = await DisplayPromptAsync(
                "Modifier le produit",
                "Prix :",
                initialValue: product.Price.ToString(),
                keyboard: Keyboard.Numeric); // ← clavier numérique pour le prix

            if (priceStr is null) return;

            var stockStr = await DisplayPromptAsync(
                "Modifier le produit",
                "Stock :",
                initialValue: product.Stock.ToString(),
                keyboard: Keyboard.Numeric);

            if (stockStr is null) return;

			// On construit le dto avec les nouvelle valeurs
			var dto = new ProductDto
			{
				Name = name.Trim(),
				Description = description.Trim(),
				Price = decimal.TryParse(priceStr, out var price) ? price : product.Price,
				Stock = int.TryParse(stockStr, out var stock) ? stock : product.Stock,
				ImageUrl = product.ImageUrl
			};

			var success = await _productService.UpdateProductAsync(productId, dto);

            if (success)
            {
                ShowStatus("Produit modifié !", isError: false);
                await LoadProductAsync(); // rafraîchit la liste
            }
            else
            {
                ShowStatus("Erreur lors de la modification.", isError: true);
            }
        }
    }

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
        ImageUrlEntry.Text = string.Empty;
    }
}