using GameStoreData.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace GameStoreData.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int GameId { get; set; }

        [Required]
        public DateTime TimeLeft { get; set; }

        [Required]
        [MaxLength(600)]
        public string Body { get; set; }

        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }

        public ICollection<Comment> ChildComments { get; set; }
    }
}