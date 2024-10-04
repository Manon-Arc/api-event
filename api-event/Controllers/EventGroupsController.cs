using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class EventGroupsController : ControllerBase
{
    private readonly EventGroupsService _eventGroupsService;
    private readonly EventsService _eventsService;

    public EventGroupsController(EventGroupsService eventGroupsService, EventsService eventsService)
    {
        _eventGroupsService = eventGroupsService;
        _eventsService = eventsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventGroupsModel>>> GetEventGroups()
    {
        var data = await _eventGroupsService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventGroupsModel>> GetEventGroup(string id)
    {
        var data = await _eventGroupsService.GetAsync(id);
        return Ok(data);
    }

    [HttpPost]
    public async void PostEventGroup([FromQuery] EventGroupsModel eventGroup)
    {
        await _eventGroupsService.CreateAsync(eventGroup);
    }

    [HttpDelete]
    public async void DeleteEventGroup(string id)
    {
        await _eventGroupsService.RemoveAsync(id);
    }

    [HttpPut("{id}")]
    public async void UpdateEventGroup(string id, [FromQuery] EventGroupsModel eventGroup)
    {
        await _eventGroupsService.UpdateAsync(id, eventGroup);
    }

    [HttpGet("{id}/events")]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents(string id)
    {
        var data = await _eventsService.GetAsync(id);
        return Ok(data);
    }
}