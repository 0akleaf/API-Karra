using APIKarra.Dtos;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace APIKarra.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly ILogger<StripeController> _logger;
    private readonly IConfiguration _configuration;

    public StripeController(ILogger<StripeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] StripeCheckoutRequestDto request)
    {
        StripeConfiguration.ApiKey = "sk_test_51RJGPVFPY9JlKYuTIWxpDYZVGOmAiqja2gP9t0g1n2spIcgITeqem4JTHITjJ0yDQ6iu0OrIiTbXHxZtaz1M73q000I0Zg2XPn";

        _logger.LogInformation($"Creating Stripe checkout session with {request.Items.Count} items");
        _logger.LogInformation($"Success URL: {request.SuccessUrl}, Cancel URL: {request.CancelUrl}");

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            CustomerEmail = request.Email,

            LineItems = request.Items.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "sek",
                    UnitAmount = (long)(item.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductName
                    },
                },
                Quantity = item.Quantity,
            }).ToList(),

            Mode = "payment",
            // Use the URLs passed from the client instead of hardcoded values
            SuccessUrl = request.SuccessUrl ?? "https://karra-blazor-test.azurewebsites.net/success",
            CancelUrl = request.CancelUrl ?? "https://karra-blazor-test.azurewebsites.net/cancel",

            Locale = "sv"
        };

        // Add shipping cost if present
        if (request.ShippingCost > 0)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "sek",
                    UnitAmount = (long)(request.ShippingCost * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Frakt ({request.ShippingMethod})"
                    },
                },
                Quantity = 1,
            });
        }

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        _logger.LogInformation($"Created Stripe session with ID: {session.Id}");
        return Ok(new { sessionId = session.Id });
    }
}
