using System.ComponentModel.DataAnnotations;

namespace MantisWebshop.Server.Sql.Models.DTOs
{
    public class CartInput
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public bool Override { get; set; }
    }
}
