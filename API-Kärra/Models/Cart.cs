namespace APIKarra.Models;

public class Cart
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public bool IsPurchased { get; set; }

}