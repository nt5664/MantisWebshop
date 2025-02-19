using System.ComponentModel.DataAnnotations;

namespace MantisWebshop.Server.Sql.Models.DTOs
{
    public class LoginInput
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

    public class SignupIntput : LoginInput 
    {
        [Required]
        [DataType(DataType.Text)]
        public string? Name { get; set; }
    }
}
