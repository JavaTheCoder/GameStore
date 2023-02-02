using GameStoreData.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStoreData.ViewModels
{
    public class GameVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Image { get; set; }

        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public IEnumerable<SelectListItem> GenresList { get; set; } = new List<SelectListItem>();

        public ICollection<int> SelectedGenreIds { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
