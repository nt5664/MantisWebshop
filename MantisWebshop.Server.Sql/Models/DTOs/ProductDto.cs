using System.Text;

namespace MantisWebshop.Server.Sql.Models.DTOs
{
    public class ProductDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public float Price { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? Creator { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("ID: {0}", Id))
                .AppendLine(string.Format("Title: {0}", Name))
                .AppendLine(string.Format("Description: {0}", Description))
                .AppendLine(string.Format("Price: {0}", Price));

            return sb.ToString();
        }
    }
}
