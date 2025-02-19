namespace MantisWebshop.Server.Sql.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public User? User { get; set; }
        public ICollection<ProductSnapshot>? ProductSnapshots { get; set; }
    }
}
