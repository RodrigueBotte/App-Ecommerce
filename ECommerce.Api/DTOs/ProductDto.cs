namespace ECommerce.Api.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // ImageUrl est un champ de type IFormFile pour permettre le téléchargement d'une image lors de la création ou de la mise à jour d'un produit.
        public IFormFile? Image { get; set; }

        // Info jeu
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int MinAge { get; set; }
        public int GameDuration { get; set; }

        // Relations autres infos
        public int PublisherId { get; set; }
        public List<int> AuthorIds { get; set; } = [];
        public List<int> CategoryIds { get; set; } = [];
        public List<int> ThemeIds { get; set; } = [];
    }
}
