using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Mobile.Models
{
    public class ProductDto
    {
        // Données envoyées pour créer ou modifier un produit
        // Pas d'Id car la BDD le génère automatiquement
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Infos jeu de société
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int MinAge { get; set; }
        public int GameDuration { get; set; }

        // Relations
        public int PublisherId { get; set; }
        public List<int> AuthorIds { get; set; } = [];
        public List<int> CategoryIds { get; set; } = [];
        public List<int> ThemeIds { get; set; } = [];
    }
}
