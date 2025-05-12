using APIKarra.Data;
using APIKarra.Models;
using Microsoft.EntityFrameworkCore;

namespace APIKarra.Services;

public class CartService
{
    private readonly KarraDbContext _dbContext;

    public CartService(KarraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Cart>> GetAllAsync()
    {
        return await _dbContext.Carts.ToListAsync();
    }

    public async Task<IEnumerable<Cart?>> GetPendingAsync()
    {
        return await _dbContext.Carts
            .Where(c => !c.IsPurchased)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cart?>> GetPurchasedAsync()
    {
        return await _dbContext.Carts
            .Where(c => c.IsPurchased)
            .ToListAsync();
    }

    public async Task AddAsync(Cart cart)
    {
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();
    }

    //behövs en UpdateAsync???

    public async Task DeleteAsync(int id)
    {
        var cart = await _dbContext.Carts.FindAsync(id);
        if (cart != null)
        {
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();
        }
    }

}