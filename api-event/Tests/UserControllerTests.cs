using Xunit;
using Moq;
using api_event.Controllers;
using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

public class UserControllerTests
{
    private readonly Mock<UsersService> _mockUsersService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUsersService = new Mock<UsersService>();
        _controller = new UserController(_mockUsersService.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsOkResult_WithListOfUsers()
    {
        // Arrange
        var mockUsers = new List<UserModel>
        {
            new UserModel { Id = "1", FirstName = "John", LastName = "Doe", Mail = "john@example.com" },
            new UserModel { Id = "2", FirstName = "Jane", LastName = "Smith", Mail = "jane@example.com" }
        };
        _mockUsersService.Setup(service => service.GetAsync()).ReturnsAsync(mockUsers);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnUsers = Assert.IsType<List<UserModel>>(okResult.Value);
        Assert.Equal(mockUsers.Count, returnUsers.Count);
    }

    [Fact]
    public async Task GetUser_ReturnsOkResult_WithUser()
    {
        // Arrange
        var userId = "1";
        var mockUser = new UserModel { Id = userId, FirstName = "John", LastName = "Doe", Mail = "john@example.com" };
        _mockUsersService.Setup(service => service.GetAsync(userId)).ReturnsAsync(mockUser);

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnUser = Assert.IsType<UserModel>(okResult.Value);
        Assert.Equal(userId, returnUser.Id);
    }

    [Fact]
    public async Task PostUser_ReturnsOkResult_WithNewUser()
    {
        // Arrange
        var userDto = new CreateUserDto { FirstName = "John", LastName = "Doe", Mail = "john@example.com" };
        var newUser = new UserModel
        {
            Id = "1",
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Mail = userDto.Mail,
            Permission = 1
        };

        _mockUsersService.Setup(service => service.CreateAsync(It.IsAny<UserModel>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PostUser(userDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var createdUser = Assert.IsType<UserModel>(okResult.Value);
        Assert.Equal(userDto.FirstName, createdUser.FirstName);
        Assert.Equal(userDto.LastName, createdUser.LastName);
        Assert.Equal(userDto.Mail, createdUser.Mail);
        Assert.Equal(1, createdUser.Permission);
    }

    [Fact]
    public async Task PutUser_ReturnsNoContentResult()
    {
        // Arrange
        var userId = "1";
        var updatedUser = new UserModel { Id = userId, FirstName = "John", LastName = "Doe", Mail = "john@example.com" };

        _mockUsersService.Setup(service => service.UpdateAsync(userId, updatedUser)).Returns(Task.CompletedTask);

        // Act
        await _controller.PutUser(userId, updatedUser);

        // Assert
        _mockUsersService.Verify(service => service.UpdateAsync(userId, updatedUser), Times.Once);
    }
}
