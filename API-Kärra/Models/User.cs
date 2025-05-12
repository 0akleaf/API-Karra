using Microsoft.AspNetCore.Identity;

namespace APIKarra.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ZIPCode { get; set; }
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

