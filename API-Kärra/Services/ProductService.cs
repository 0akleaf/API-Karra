using APIKarra.Data;
using APIKarra.Models;
using Microsoft.EntityFrameworkCore;

namespace APIKarra.Services;

public class ProductService
{
    private readonly KarraDbContext _dbContext;

    public ProductService(KarraDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbContext.Products.ToListAsync();
    }
    public async Task<Product?> GetByIdAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        return product;
    }
    public async Task<IEnumerable<Product?>> GetByNameAsync(string name)
    {
        var products = await _dbContext.Products
            .Where(p => p.Name.Contains(name))
            .ToListAsync();

        return products;
    }

    public async Task AddAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Product updatedProduct)
    {
        var product = await _dbContext.Products.FindAsync(id);

        product.Name = ShouldUpdate(updatedProduct.Name) ? updatedProduct.Name : product.Name;
        product.Company = ShouldUpdate(updatedProduct.Company) ? updatedProduct.Company : product.Company;
        product.Description = ShouldUpdate(updatedProduct.Description) ? updatedProduct.Description : product.Description;
        product.Category = ShouldUpdate(updatedProduct.Category) ? updatedProduct.Category : product.Category;
        product.Genre = ShouldUpdate(updatedProduct.Genre) ? updatedProduct.Genre : product.Genre;
        product.Price = updatedProduct.Price > 0 ? updatedProduct.Price : product.Price;
        product.ImageUrl = ShouldUpdate(updatedProduct.ImageUrl) ? updatedProduct.ImageUrl : product.ImageUrl;
        product.Stock = updatedProduct.Stock >= 0 ? updatedProduct.Stock : product.Stock;
        product.PGRating = updatedProduct.PGRating > 0 ? updatedProduct.PGRating : product.PGRating;

        await _dbContext.SaveChangesAsync();
    }

    private static bool ShouldUpdate(string? value)
    {
        return !string.IsNullOrEmpty(value) && value != "string";
    }
    public async Task DeleteAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
    }

}