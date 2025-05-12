using APIKarra.Data;
using APIKarra.Services;
using APIKarra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIKarra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            var carts = await _cartService.GetAllAsync();
            return Ok(carts);
        }

        // GET: api/Cart/pending
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetPendingCarts()
        {
            var carts = await _cartService.GetPendingAsync();
            return Ok(carts);
        }

        // GET: api/Cart/purchased
        [HttpGet("purchased")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetPurchasedCarts()
        {
            var carts = await _cartService.GetPurchasedAsync();
            return Ok(carts);
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var carts = await _cartService.GetAllAsync();
            var cart = carts.FirstOrDefault(c => c.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.Id)
            {
                return BadRequest();
            }

            // Note: Behövs en update i service, sparar direkt tillfälligt
            try
            {
                await _cartService.AddAsync(cart); // Detta bör vara uppdate eventuellt
            }
            catch
            {
                if (!await CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            await _cartService.AddAsync(cart);
            return CreatedAtAction("GetCart", new { id = cart.Id }, cart);
        }

        // DELETE: api/Cart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            await _cartService.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> CartExists(int id)
        {
            var carts = await _cartService.GetAllAsync();
            return carts.Any(e => e.Id == id);
        }
    }
}