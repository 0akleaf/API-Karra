using APIKarra.Data;
using APIKarra.Models;
using System.Text.Json;

public class DataSeeder
{
    private readonly KarraDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(KarraDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedDataAsync()
    {
        try
        {
            await SeedProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task SeedProductsAsync()
    {
        if (!_context.Products.Any())
        {
            _logger.LogInformation("Seeding products...");

            var jsonData = File.ReadAllText("steam_games.json");
            var products = JsonSerializer.Deserialize<List<Product>>(jsonData);

            foreach (var product in products)
            {
                // Data conversions and adjustments
                product.Price = (int)Math.Round(product.Price);
                product.Stock = product.Stock == -1 ? 0 : product.Stock;

                // Initialize Ratings collection
                product.Ratings = new List<Rating>();

                _context.Products.Add(product);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} products.", products.Count);
        }
    }
}