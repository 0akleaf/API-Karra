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
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        // Get all events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventService.GetEventsAsync();
            return Ok(events);
        }

        // Get an event by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return Ok(eventItem);
        }

        // Create a new event
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest("Event data is required.");
            }

            // Model Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate Capacity and TicketsAvailable
            if (newEvent.Capacity.HasValue && newEvent.TicketsAvailable.HasValue && newEvent.TicketsAvailable > newEvent.Capacity)
            {
                return BadRequest("Tickets available cannot exceed capacity.");
            }

            await _eventService.AddEventAsync(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

        // Update an event
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
        {
            if (updatedEvent == null || updatedEvent.Id != id)
            {
                return BadRequest("Event ID mismatch.");
            }

            // Model Validation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate Capacity and TicketsAvailable
            if (updatedEvent.Capacity.HasValue && updatedEvent.TicketsAvailable.HasValue && updatedEvent.TicketsAvailable > updatedEvent.Capacity)
            {
                return BadRequest("Tickets available cannot exceed capacity.");
            }

            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            await _eventService.UpdateEventAsync(updatedEvent);
            return NoContent();
        }

        // Delete an event
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }
    }
}
