using System.Text.Json.Serialization;

namespace APIKarra.Models;

public class OrderProduct
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    
    [JsonIgnore]
    public Order Order { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
}