using System.Text.Json.Serialization;

namespace APIKarra.Models;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    
    [JsonIgnore]
    public User User { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}

