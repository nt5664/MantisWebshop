namespace MantisWebshop.Server.Sql.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public float Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Version { get; set; }

        public User? Creator { get; set; }
    }
}
