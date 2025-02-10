using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text;

namespace MantisWebshop.Server.Models
{
    public class Product
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonIgnore]
        public string IdString
        {
            get => Id.ToString();
            set 
            {
                IsNew = string.IsNullOrWhiteSpace(value);
                Id = IsNew ? new ObjectId() : new ObjectId(value); 
            }
        }

        [BsonIgnore]
        public bool IsNew { get; private set; } = false;

        [BsonElement("userId")]
        public ObjectId UserId { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("imageUrl")]
        public string? Image { get; set; }

        [BsonElement("__v")]
        public int Version { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("***** Product Data *****");
            foreach (var prop in GetType().GetProperties())
            {
                sb.AppendLine($"{prop.Name}: {prop.GetValue(this)}");
            }

            sb.AppendLine("***** End Product Data *****");
            return sb.ToString();
        }
    }
}
