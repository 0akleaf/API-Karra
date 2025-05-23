using System.ComponentModel.DataAnnotations;

namespace APIKarra.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
