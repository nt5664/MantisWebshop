namespace MantisWebshop.Server.Sql.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }
        public required Product Product { get; set; }

        public User? User { get; set; }
    }
}
