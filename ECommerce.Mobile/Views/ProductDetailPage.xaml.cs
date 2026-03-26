namespace ECommerce.Mobile.Views;

public partial class ProductDetailPage : ContentPage
{
	public ProductDetailPage()
	{
		InitializeComponent();
	}

	public ProductDetailPage(Models.Product product): this()
	{
		BindingContext = product;
		BuildTags(CategoriesLayout, product.Categories.Select(c => c.Name).ToList());
        BuildTags(ThemesLayout, product.Themes.Select(t => t.Name).ToList());
        BuildTags(AuthorsLayout, product.Authors.Select(a => a.Name).ToList());
    }

	private void BuildTags(FlexLayout layout, List<string> items)
	{
		if (items.Count == 0)
		{
			layout.Add(new Label
			{
				Text = "Non renseigné",
				FontSize = 13,
				TextColor = Colors.Gray
			});
			return;
		}

		foreach (var item in items)
		{
			layout.Add(new Border
			{
				BackgroundColor = Color.FromArgb("#FFF0F0"),
				Stroke = Color.FromArgb("#IndianRed"),
				StrokeThickness = 1,
				Padding = new Thickness(10, 4),
				Margin = new Thickness(0, 0, 6, 6),
				StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
				Content = new Label
				{
					Text = item,
					FontSize = 12,
					TextColor = Color.FromArgb("#CD5C5C")
				}
			});
		}
	}
}