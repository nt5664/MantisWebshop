namespace MantisWebshop.Server.Models
{
    public class CartItemDto
    {
        public required Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
