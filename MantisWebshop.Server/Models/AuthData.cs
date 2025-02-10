using System.ComponentModel.DataAnnotations;

namespace MantisWebshop.Server.Models
{
    public struct AuthData
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
