using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using api_event.Models;
using api_event.Services;
using MongoDB.Bson;

namespace api_event.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventsService _eventsService;

        public EventController(EventsService eventsService)
        {
            _eventsService = eventsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var data = await _eventsService.GetAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(string id)
        {
            var data = await _eventsService.GetAsync(id);
            return Ok(data);
        }

        /// <summary>
        /// Creates a new event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST /Event
        ///     {
        ///         "name": "Event Name"
        ///     }
        /// </remarks>
        /// <param name="eventDto">The event data to create the event</param>
        /// <returns>The newly created event</returns>
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent([FromQuery] CreateEventDto eventDto)
        {
            var newEvent = new Event
            {
                Name = eventDto.Name,
                Date = DateTimeOffset.UtcNow // Example, you can modify this as necessary
            };

            await _eventsService.CreateAsync(newEvent);
            return Ok(newEvent);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(string id)
        {
            await _eventsService.RemoveAsync(id);
            return NoContent();
        }
    }

    [HttpPut("{id}")]
    public async void PutEvent(string id, [FromQuery] EventModel eventModelData)
    {
        await eventsService.UpdateAsync(id, eventModelData);
    }
        
}
