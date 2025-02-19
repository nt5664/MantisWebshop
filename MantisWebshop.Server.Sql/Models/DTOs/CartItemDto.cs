namespace MantisWebshop.Server.Sql.Models.DTOs
{
    public class CartItemDto
    {
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
