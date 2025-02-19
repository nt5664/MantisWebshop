namespace MantisWebshop.Server.Sql.Models.DTOs
{
    public class OrderDto
    {
        public required string Id { get; set; }
        public float TotalPrice { get; set; }
        public required ICollection<ProductSnapshotDto> Products { get; set; }
    }
}
