using System.ComponentModel.DataAnnotations;

namespace Dental_Surgery.Pages.PageViewModels
{
    public class Login
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
