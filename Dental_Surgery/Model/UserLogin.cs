using System.ComponentModel.DataAnnotations;

namespace Dental_Surgery.Model
{
    public class UserLogin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
