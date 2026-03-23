using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string ImageUrl {  get; set; } = string.Empty;
        public int Stock { get; set; }

        // Informations supplémentaires
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int MinAge { get; set; }
        public int GameDuration { get; set; }

        // Relation many to one avec Publisher (un éditeur par jeu)
        public int PublisherId { get; set; }
        public Publisher Publishers { get; set; } = null!;

        // Relation many to many ( plusieurs auteurs, catégories et theme possibles)
        public ICollection<Author> Authors { get; set; } = [];
        public ICollection<Category> Categories { get; set; } = [];
        public ICollection<Theme> Themes { get; set; } = [];
    }
}
