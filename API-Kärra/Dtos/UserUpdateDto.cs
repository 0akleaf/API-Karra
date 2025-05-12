using System.ComponentModel.DataAnnotations;

namespace APIKarra.Dtos
{
    public class UserUpdateDto
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        
        public string? Address { get; set; }
        
        public string? City { get; set; }
        
        public string? ZipCode { get; set; }
        
        public bool EmailConfirmed { get; set; }
        
        public string? PhoneNumber { get; set; }
    }
}