using APIKarra.Data;
using APIKarra.Services;
using APIKarra.Models;
using APIKarra.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace APIKarra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Order
        // [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            
            // Explicitly load products for each OrderProduct if they're not already loaded
            if (order.OrderProducts != null)
            {
                foreach (var orderProduct in order.OrderProducts)
                {
                    if (orderProduct.Product == null && orderProduct.ProductId > 0)
                    {
                        // Load the product if it's not already loaded
                        var product = await _orderService.GetProductByIdAsync(orderProduct.ProductId);
                        orderProduct.Product = product;
                    }
                }
            }
            
            return order;
        }

        // GET: api/Order/byuser/{userId}
        [HttpGet("byuser/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(string userId)
        {
            var orders = await _orderService.GetByCustomerAsync(userId);
            
            // Explicitly ensure products are loaded for each order
            foreach (var order in orders)
            {
                if (order.OrderProducts != null)
                {
                    foreach (var orderProduct in order.OrderProducts)
                    {
                        if (orderProduct.Product == null && orderProduct.ProductId > 0)
                        {
                            // Load the product if it's not already loaded
                            var product = await _orderService.GetProductByIdAsync(orderProduct.ProductId);
                            orderProduct.Product = product;
                        }
                    }
                }
            }
            
            return Ok(orders);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderCreateDto dto)
        {
            var order = await _orderService.AddAsync(dto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        } 

        // DELETE: api/Order/5
        // [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteAsync(id);
            return NoContent();
        }

        // [Authorize(Roles = "ADMIN")]
        [HttpGet("overview")]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrderOverview()
        {
            var orders = await _orderService.GetOrderSummaryAsync();
            return Ok(orders);
        }
    }
}