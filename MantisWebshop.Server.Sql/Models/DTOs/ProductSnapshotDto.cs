namespace MantisWebshop.Server.Sql.Models.DTOs
{
    public class ProductSnapshotDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public required string ProductId { get; set; }
    }
}
