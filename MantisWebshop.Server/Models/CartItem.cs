using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MantisWebshop.Server.Models
{
    public class CartItem
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("productId")]
        public ObjectId ProductId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }
    }
}
