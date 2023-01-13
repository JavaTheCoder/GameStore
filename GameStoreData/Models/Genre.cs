using System.ComponentModel.DataAnnotations;

namespace GameStoreData.Models
{
    /// <summary>
    /// Genre ViewModel
    /// </summary>
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
