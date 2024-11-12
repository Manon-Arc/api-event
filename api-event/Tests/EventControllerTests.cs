using Xunit;
using Moq;
using api_event.Controllers;
using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

public class EventControllerTests
{
    private readonly Mock<EventsService> _mockEventsService;
    private readonly EventController _controller;

    public EventControllerTests()
    {
        _mockEventsService = new Mock<EventsService>();
        _controller = new EventController(_mockEventsService.Object);
    }

    [Fact]
    public async Task GetEvents_ReturnsOkResult_WithListOfEvents()
    {
        // Arrange
        var mockEvents = new List<EventModel>
        {
            new EventModel { Id = "1", Name = "Event 1", Date = DateTimeOffset.UtcNow },
            new EventModel { Id = "2", Name = "Event 2", Date = DateTimeOffset.UtcNow }
        };
        _mockEventsService.Setup(service => service.GetAsync()).ReturnsAsync(mockEvents);

        // Act
        var result = await _controller.GetEvents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnEvents = Assert.IsType<List<EventModel>>(okResult.Value);
        Assert.Equal(mockEvents.Count, returnEvents.Count);
    }

    [Fact]
    public async Task GetEvent_ReturnsOkResult_WithEvent()
    {
        // Arrange
        var eventId = "1";
        var mockEvent = new EventModel { Id = eventId, Name = "Event 1", Date = DateTimeOffset.UtcNow };
        _mockEventsService.Setup(service => service.GetAsync(eventId)).ReturnsAsync(mockEvent);

        // Act
        var result = await _controller.GetEvent(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnEvent = Assert.IsType<EventModel>(okResult.Value);
        Assert.Equal(eventId, returnEvent.Id);
    }

    [Fact]
    public async Task PostEvent_ReturnsOkResult_WithNewEvent()
    {
        // Arrange
        var eventDto = new CreateEventDto { Name = "New Event" };
        var newEvent = new EventModel { Id = "1", Name = eventDto.Name, Date = DateTimeOffset.UtcNow };

        _mockEventsService.Setup(service => service.CreateAsync(It.IsAny<EventModel>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PostEvent(eventDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var createdEvent = Assert.IsType<EventModel>(okResult.Value);
        Assert.Equal(eventDto.Name, createdEvent.Name);
    }

    [Fact]
    public async Task DeleteEvent_ReturnsNoContentResult()
    {
        // Arrange
        var eventId = "1";
        _mockEventsService.Setup(service => service.RemoveAsync(eventId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteEvent(eventId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutEvent_ReturnsOkResult_WithUpdatedEvent()
    {
        // Arrange
        var eventId = "1";
        var updatedEvent = new EventModel { Id = eventId, Name = "Updated Event", Date = DateTimeOffset.UtcNow };

        _mockEventsService.Setup(service => service.UpdateAsync(eventId, updatedEvent)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PutEvent(eventId, updatedEvent);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, ((OkResult)okResult).StatusCode);
    }
}
