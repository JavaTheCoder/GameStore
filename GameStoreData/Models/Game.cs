using GameStoreData.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStoreData.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Image { get; set; }

        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

        [NotMapped]
        public CommentVM CommentVM { get; set; }
    }
}
