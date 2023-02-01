using GameStoreData.Models;

namespace GameStoreData.ViewModels
{
    public class CommentVM
    {
        public int CommentVMId { get; set; }

        public int? ParentCommentId { get; set; }

        public int GameId { get; set; }

        public string? UserId { get; set; }

        public string Body { get; set; }

        public ICollection<Comment> GameComments { get; set; } = new List<Comment>();
    }
}