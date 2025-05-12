namespace APIKarra.Models;

public class OrderSummary
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public double TotalPrice { get; set; }
    public string ProductsPurchased { get; set; } = string.Empty;
}