using Xunit;
using Moq;
using api_event.Controllers;
using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

public class EventGroupsControllerTests
{
    private readonly Mock<EventGroupsService> _mockEventGroupsService;
    private readonly Mock<LinkEventToGroupService> _mockLinkEventToGroupService;
    private readonly EventGroupsController _controller;

    public EventGroupsControllerTests()
    {
        _mockEventGroupsService = new Mock<EventGroupsService>();
        _mockLinkEventToGroupService = new Mock<LinkEventToGroupService>();
        _controller = new EventGroupsController(_mockEventGroupsService.Object, _mockLinkEventToGroupService.Object);
    }

    [Fact]
    public async Task GetEventGroups_ReturnsOkResult_WithListOfEventGroups()
    {
        // Arrange
        var mockEventGroups = new List<EventGroupsModel>
        {
            new EventGroupsModel { Id = "1", Name = "Group 1" },
            new EventGroupsModel { Id = "2", Name = "Group 2" }
        };
        _mockEventGroupsService.Setup(service => service.GetAsync()).ReturnsAsync(mockEventGroups);

        // Act
        var result = await _controller.GetEventGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnEventGroups = Assert.IsType<List<EventGroupsModel>>(okResult.Value);
        Assert.Equal(mockEventGroups.Count, returnEventGroups.Count);
    }

    [Fact]
    public async Task GetEventGroup_ReturnsOkResult_WithEventGroup()
    {
        // Arrange
        var groupId = "1";
        var mockEventGroup = new EventGroupsModel { Id = groupId, Name = "Group 1" };
        _mockEventGroupsService.Setup(service => service.GetAsync(groupId)).ReturnsAsync(mockEventGroup);

        // Act
        var result = await _controller.GetEventGroup(groupId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnEventGroup = Assert.IsType<EventGroupsModel>(okResult.Value);
        Assert.Equal(groupId, returnEventGroup.Id);
    }

    [Fact]
    public async Task PostEventGroups_ReturnsOkResult_WithNewEventGroup()
    {
        // Arrange
        var eventDto = new CreateEventGroupsDto { Name = "New Group" };
        var newEventGroup = new EventGroupsModel { Id = "1", Name = eventDto.Name };

        _mockEventGroupsService.Setup(service => service.CreateAsync(It.IsAny<EventGroupsModel>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PostEventGroups(eventDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var createdEventGroup = Assert.IsType<EventGroupsModel>(okResult.Value);
        Assert.Equal(eventDto.Name, createdEventGroup.Name);
    }

    [Fact]
    public async Task PostLinkEventGroup_ReturnsOkResult()
    {
        // Arrange
        var groupId = "1";
        var eventId = "10";

        _mockLinkEventToGroupService.Setup(service => service.CreateAsync(It.IsAny<LinkEventToGroupModel>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PostLinkEventGroup(groupId, eventId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteEventGroup_ReturnsNoContentResult()
    {
        // Arrange
        var groupId = "1";
        _mockEventGroupsService.Setup(service => service.RemoveAsync(groupId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteEventGroup(groupId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteLinkEventGroup_ReturnsNoContentResult()
    {
        // Arrange
        var groupId = "1";
        var linkId = "10";
        _mockLinkEventToGroupService.Setup(service => service.RemoveAsync(linkId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteLinkEventGroup(groupId, linkId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateEventGroup_ReturnsOkResult_WithUpdatedEventGroup()
    {
        // Arrange
        var groupId = "1";
        var updatedEventGroup = new EventGroupsModel { Id = groupId, Name = "Updated Group" };

        _mockEventGroupsService.Setup(service => service.UpdateAsync(groupId, updatedEventGroup)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateEventGroup(groupId, updatedEventGroup);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnEventGroup = Assert.IsType<EventGroupsModel>(okResult.Value);
        Assert.Equal(updatedEventGroup.Name, returnEventGroup.Name);
    }
}
