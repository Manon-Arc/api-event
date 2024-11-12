using Xunit;
using Moq;
using api_event.Controllers;
using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

public class TicketOfficeControllerTests
{
    private readonly Mock<TicketOfficeService> _mockTicketOfficeService;
    private readonly TicketOfficeController _controller;

    public TicketOfficeControllerTests()
    {
        _mockTicketOfficeService = new Mock<TicketOfficeService>();
        _controller = new TicketOfficeController(_mockTicketOfficeService.Object);
    }

    [Fact]
    public async Task GetTickets_ReturnsOkResult_WithListOfTickets()
    {
        // Arrange
        var mockTickets = new List<TicketOfficeModel>
        {
            new TicketOfficeModel { Id = "1", Name = "Main Event", Id_event = "E1", Nbr_ticket = 100, Price = 50.0f },
            new TicketOfficeModel { Id = "2", Name = "Secondary Event", Id_event = "E2", Nbr_ticket = 200, Price = 75.0f }
        };
        _mockTicketOfficeService.Setup(service => service.GetAsync()).ReturnsAsync(mockTickets);

        // Act
        var result = await _controller.GetTickets();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnTickets = Assert.IsType<List<TicketOfficeModel>>(okResult.Value);
        Assert.Equal(mockTickets.Count, returnTickets.Count);
    }

    [Fact]
    public async Task GetTicket_ReturnsOkResult_WithTicket()
    {
        // Arrange
        var ticketId = "1";
        var mockTicket = new TicketOfficeModel { Id = ticketId, Name = "Main Event", Id_event = "E1", Nbr_ticket = 100, Price = 50.0f };
        _mockTicketOfficeService.Setup(service => service.GetAsync(ticketId)).ReturnsAsync(mockTicket);

        // Act
        var result = await _controller.GetTicket(ticketId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnTicket = Assert.IsType<TicketOfficeModel>(okResult.Value);
        Assert.Equal(ticketId, returnTicket.Id);
    }

    [Fact]
    public async Task PostTicket_ReturnsCreatedAtAction_WithNewTicket()
    {
        // Arrange
        var newTicket = new TicketOfficeModel
        {
            Id = "1",
            Name = "Main Event",
            Id_event = "E1",
            Nbr_ticket = 100,
            Price = 50.0f
        };

        _mockTicketOfficeService.Setup(service => service.CreateAsync(newTicket)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PostTicket(newTicket);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdTicket = Assert.IsType<TicketOfficeModel>(createdAtActionResult.Value);
        Assert.Equal(newTicket.Id, createdTicket.Id);
    }

    [Fact]
    public async Task PostTicket_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var invalidTicket = new TicketOfficeModel
        {
            Name = "", // Invalid due to empty Name
            Id_event = "E1",
            Nbr_ticket = 100,
            Price = -50.0f // Invalid price
        };
        _controller.ModelState.AddModelError("Price", "Price must be a positive value.");

        // Act
        var result = await _controller.PostTicket(invalidTicket);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteEvent_ReturnsNotFound_WhenTicketNotFound()
    {
        // Arrange
        var ticketId = "1";
        _mockTicketOfficeService.Setup(service => service.RemoveAsync(ticketId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteEvent(ticketId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateEventGroup_ReturnsOkResult_WhenTicketUpdated()
    {
        // Arrange
        var ticketId = "1";
        var updatedTicket = new TicketOfficeModel
        {
            Id = ticketId,
            Name = "Updated Event",
            Id_event = "E1",
            Nbr_ticket = 150,
            Price = 60.0f
        };

        _mockTicketOfficeService.Setup(service => service.UpdateAsync(ticketId, updatedTicket)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateEventGroup(ticketId, updatedTicket);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockTicketOfficeService.Verify(service => service.UpdateAsync(ticketId, updatedTicket), Times.Once);
    }
}
