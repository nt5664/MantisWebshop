namespace MantisWebshop.Server.Sql.Models
{
    public class ProductSnapshot
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public float Price { get; set; }
        public int SnapshotVersion { get; set; }
        public Guid ProductId { get; set; }

        public Order? Order { get; set; }
    }
}
