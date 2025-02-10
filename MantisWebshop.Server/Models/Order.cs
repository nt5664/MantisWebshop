using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MantisWebshop.Server.Models
{
    public class Order
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("user")]
        public required User User { get; set; }

        [BsonElement("products")]
        public required Dictionary<Product, uint> Products { get; set; }
    }
}
