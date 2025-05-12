using APIKarra.Data;
using APIKarra.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace APIKarra.Services
{
    public class EventService
    {
        private readonly KarraDbContext _context;

        public EventService(KarraDbContext context)
        {
            _context = context;
        }

        // Get all events
        public async Task<List<Event>> GetEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        // Get an event by ID
        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        }

        // Add a new event
        public async Task AddEventAsync(Event newEvent)
        {
            if (newEvent.Capacity.HasValue && newEvent.TicketsAvailable.HasValue && newEvent.TicketsAvailable > newEvent.Capacity)
            {
                throw new InvalidOperationException("Tickets available cannot exceed capacity.");
            }

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
        }

        // Update an event
        public async Task UpdateEventAsync(Event updatedEvent)
        {
            if (updatedEvent.Capacity.HasValue && updatedEvent.TicketsAvailable.HasValue && updatedEvent.TicketsAvailable > updatedEvent.Capacity)
            {
                throw new InvalidOperationException("Tickets available cannot exceed capacity.");
            }

            _context.Events.Update(updatedEvent);
            await _context.SaveChangesAsync();
        }

        // Delete an event -
        public async Task DeleteEventAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete != null)
            {
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
