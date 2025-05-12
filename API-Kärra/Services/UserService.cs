using APIKarra.Data;
using APIKarra.Models;
using Microsoft.EntityFrameworkCore;

namespace APIKarra.Services;
public class UserService
{
    private readonly KarraDbContext _dbContext;

    public UserService(KarraDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }
    public async Task<User?> GetByIdAsync(string id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        return user;
    }
    public async Task<IEnumerable<User>> GetByEmailAsync(string email)
    {
        var user = await _dbContext.Users
            .Where(c => c.Email.Contains(email))
            .ToListAsync();

        return user;
    }

    public async Task AddAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdateAsync(string id, User updaterUser)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found");

        user.FirstName = ShouldUpdate(updaterUser.FirstName) ? updaterUser.FirstName : user.FirstName;
        user.LastName = ShouldUpdate(updaterUser.LastName) ? updaterUser.LastName : user.LastName;
        user.Email = ShouldUpdate(updaterUser.Email) ? updaterUser.Email : user.Email;
        user.UserName = ShouldUpdate(updaterUser.UserName) ? updaterUser.UserName : user.UserName;
        user.PhoneNumber = ShouldUpdate(updaterUser.PhoneNumber) ? updaterUser.PhoneNumber : user.PhoneNumber;
        user.Address = updaterUser.Address;
        
        // Allow updating email confirmation status
        if (updaterUser.EmailConfirmed != user.EmailConfirmed)
        {
            user.EmailConfirmed = updaterUser.EmailConfirmed;
        }

        await _dbContext.SaveChangesAsync();
    }

    private bool ShouldUpdate(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "string";
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

}