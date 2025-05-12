namespace APIKarra.Dtos;

public class StripeCheckoutRequestDto
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public string Email { get; set; } = string.Empty;
    public double ShippingCost { get; set; } = 0;
    public string ShippingMethod { get; set; } = "Standard";
    public string? SuccessUrl { get; set; }
    public string? CancelUrl { get; set; }
}


