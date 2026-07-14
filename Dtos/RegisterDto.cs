using System.ComponentModel.DataAnnotations;

namespace article.Dtos
{
    public class RegisterDto
    {

        [Required(ErrorMessage ="Username is required"),MinLength(3),MaxLength(50),]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required"),EmailAddress] public string EmailAddress { get; set;} = string.Empty;
        [Required(ErrorMessage = "Password is required"),MinLength(6)]
        public string Password { get; set; } = string.Empty;

    }
}
