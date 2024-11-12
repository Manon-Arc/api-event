using Xunit;
using Moq;
using api_event.Controllers;
using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

public class TicketControllerTests
{
    private readonly Mock<TicketsService> _mockTicketsService;
    private readonly TicketController _controller;

    public TicketControllerTests()
    {
        _mockTicketsService = new Mock<TicketsService>();
        _controller = new TicketController(_mockTicketsService.Object);
    }

    [Fact]
    public async Task GetTickets_ReturnsOkResult_WithListOfTickets()
    {
        // Arrange
        var mockTickets = new List<TicketModel>
        {
            new TicketModel { Id = "1", UserID = "User1", EventID = "Event1", ExpireDate = "2024-12-31" },
            new TicketModel { Id = "2", UserID = "User2", EventID = "Event2", ExpireDate = "2025-01-15" }
        };
        _mockTicketsService.Setup(service => service.GetAsync()).ReturnsAsync(mockTickets);

        // Act
        var result = await _controller.GetTickets();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnTickets = Assert.IsType<List<TicketModel>>(okResult.Value);
        Assert.Equal(mockTickets.Count, returnTickets.Count);
    }

    [Fact]
    public async Task GetTicket_ReturnsOkResult_WithTicket()
    {
        // Arrange
        var ticketId = "1";
        var mockTicket = new TicketModel { Id = ticketId, UserID = "User1", EventID = "Event1", ExpireDate = "2024-12-31" };
        _mockTicketsService.Setup(service => service.GetAsync(ticketId)).ReturnsAsync(mockTicket);

        // Act
        var result = await _controller.GetTicket(ticketId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnTicket = Assert.IsType<TicketModel>(okResult.Value);
        Assert.Equal(ticketId, returnTicket.Id);
    }

    [Fact]
    public async Task PostTicket_ReturnsOkResult_WithNewTicket()
    {
        // Arrange
        var ticketDto = new CreateTicketDto
        {
            UserID = "User1",
            EventID = "Event1",
            ExpireDate = "2024-12-31"
        };
        var newTicket = new TicketModel
        {
            Id = "1",
            UserID = ticketDto.UserID,
            EventID = ticketDto.EventID,
            ExpireDate = ticketDto.ExpireDate
        };
        _mockTicketsService.Setup(service => service.CreateAsync(It.IsAny<TicketModel>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PostTicket(ticketDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var createdTicket = Assert.IsType<TicketModel>(okResult.Value);
        Assert.Equal(ticketDto.UserID, createdTicket.UserID);
        Assert.Equal(ticketDto.EventID, createdTicket.EventID);
        Assert.Equal(ticketDto.ExpireDate, createdTicket.ExpireDate);
    }

    [Fact]
    public async Task DeleteEvent_CallsRemoveAsync_WithCorrectId()
    {
        // Arrange
        var ticketId = "1";
        _mockTicketsService.Setup(service => service.RemoveAsync(ticketId)).Returns(Task.CompletedTask);

        // Act
        _controller.DeleteEvent(ticketId);

        // Assert
        _mockTicketsService.Verify(service => service.RemoveAsync(ticketId), Times.Once);
    }
}
