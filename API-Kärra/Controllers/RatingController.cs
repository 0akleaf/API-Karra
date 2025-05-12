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
    public class RatingController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // GET: api/Rating
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
        {
            var ratings = await _ratingService.GetAllAsync();
            return Ok(ratings);
        }

        // GET: api/Rating/byproduct/5
        [HttpGet("byproduct/{productId}")]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatingsByProduct(int productId)
        {
            var ratings = await _ratingService.GetByProductAsync(productId);
            return Ok(ratings);
        }

        // POST: api/Rating
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating)
        {
            await _ratingService.AddAsync(rating);
            return CreatedAtAction(nameof(GetRatings), new { id = rating.Id }, rating);
        }

        // DELETE: api/Rating/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            await _ratingService.DeleteAsync(id);
            return NoContent();
        }
    }
}
