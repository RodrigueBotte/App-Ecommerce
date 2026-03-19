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
	}
}