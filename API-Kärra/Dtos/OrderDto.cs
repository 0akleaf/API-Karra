public class OrderCreateDto
{
    public string UserId { get; set; }
    public List<OrderProductDto> Products { get; set; }
}

public class OrderProductDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}