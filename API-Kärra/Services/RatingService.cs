using APIKarra.Data;
using APIKarra.Models;
using Microsoft.EntityFrameworkCore;

namespace APIKarra.Services;

public class RatingService
{
    private readonly KarraDbContext _dbContext;

    public RatingService(KarraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Rating>> GetAllAsync()
    {
        return await _dbContext.Ratings.ToListAsync();
    }

    public async Task<IEnumerable<Rating>> GetByProductAsync(int productId)
    {
        return await _dbContext.Ratings
        .Where(r => r.ProductId == productId)
        .ToListAsync();
    }

    public async Task AddAsync(Rating rating)
    {
        _dbContext.Ratings.Add(rating);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rating = await _dbContext.Ratings.FindAsync(id);
        _dbContext.Ratings.Remove(rating);
        await _dbContext.SaveChangesAsync();
    }
}