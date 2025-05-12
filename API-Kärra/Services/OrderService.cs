using APIKarra.Data;
using APIKarra.Models;
using Microsoft.EntityFrameworkCore;

namespace APIKarra.Services;

public class OrderService
{
    private readonly KarraDbContext _dbContext;

    public OrderService(KarraDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _dbContext.Orders
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .ToListAsync();
    }
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    public async Task<IEnumerable<Order>> GetByCustomerAsync(string userId)
    {
        return await _dbContext.Orders
        .Where(o => o.UserId == userId)
        .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
        .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await _dbContext.Products.FindAsync(productId);
    }

    public async Task<Order> AddAsync(OrderCreateDto dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            OrderDate = DateTime.Now
        };

        order.OrderProducts = dto.Products.Select(p => new OrderProduct
        {
            ProductId = p.ProductId,
            Quantity = p.Quantity,
            Order = order
        }).ToList();

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order != null)
        {
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<OrderSummary>> GetOrderSummaryAsync()
    {
        var summaries = await (from order in _dbContext.Orders
                               join user in _dbContext.Users on order.UserId equals user.Id
                               select new OrderSummary
                               {
                                   FullName = user.FirstName + " " + user.LastName,
                                   Email = user.Email,
                                   Address = user.Address,
                                   OrderId = order.Id,
                                   OrderDate = order.OrderDate,
                                   TotalPrice = order.OrderProducts
                                       .Sum(op => op.Product.Price * op.Quantity),
                                   ProductsPurchased = string.Join(", ",
                                       order.OrderProducts.Select(op =>
                                           $"{op.Product.Name} (x{op.Quantity})"))
                               }).ToListAsync();

        return summaries;
    }


}
