using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MantisWebshop.Server.Models
{
    public class User
    {
        public class UserCart
        {
            [BsonElement("items")]
            public List<CartItem> Items { get; private set; } = new List<CartItem>();
        }

        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("email")]
        [BsonRequired]
        public string Email { get; set; }

        [BsonElement("password")]
        [BsonRequired]
        public string Password { get; set; }

        [BsonElement("cart")]
        public UserCart? Cart { get; set; }
    }
}
