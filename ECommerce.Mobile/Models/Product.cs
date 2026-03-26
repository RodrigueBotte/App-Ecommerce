namespace ECommerce.Mobile.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = "";
        public int Stock { get; set; }

        // Infos jeu de société
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int MinAge { get; set; }
        public int GameDuration { get; set; }

        // Relations
        public int PublisherId { get; set; }
        public Publisher? Publishers { get; set; }
        public List<Author> Authors { get; set; } = [];
        public List<Category> Categories { get; set; } = [];
        public List<Theme> Themes { get; set; } = [];
    }
}
