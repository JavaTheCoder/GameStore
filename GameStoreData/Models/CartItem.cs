namespace GameStoreData.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Game GameInCart { get; set; }
        public int Quantity { get; set; }
    }
}
