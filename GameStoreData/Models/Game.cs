using System.ComponentModel.DataAnnotations;

namespace GameStoreData.Models
{
    /// <summary>
    /// Game ViewModel
    /// </summary>
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }
    }
}
