using System.Text.Json.Serialization;

namespace APIKarra.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Company { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string Genre { get; set; }
    public double Price { get; set; }
    public string ImageUrl { get; set; }
    public int Stock { get; set; }
    public int PGRating { get; set; }
    
    [JsonIgnore]
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}

