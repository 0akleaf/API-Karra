using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIKarra.Models;

public class Rating
{
    public int Id { get; set; }
    [Range(1, 5)]
    public int RatingValue { get; set; }
    public DateTime Date { get; set; }
    public int ProductId { get; set; }
    
    [JsonIgnore]
    public Product Product { get; set; } = null!;
    
    public string UserId { get; set; }
    
    [JsonIgnore]
    public User User { get; set; } = null!;
}

