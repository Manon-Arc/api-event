using Xunit;
using Moq;
using api_event.Controllers;
using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

public class CredentialControllerTests
{
    private readonly Mock<CredentialsService> _mockCredentialService;
    private readonly CredentialController _controller;

    public CredentialControllerTests()
    {
        _mockCredentialService = new Mock<CredentialsService>();
        _controller = new CredentialController(_mockCredentialService.Object);
    }

    [Fact]
    public async Task Register_ReturnsOkResult_WhenSuccessful()
    {
        // Arrange
        var credentials = new CredentialsModel
        {
            Mail = "test@mail.com",
            Password = "password123"
        };

        _mockCredentialService.Setup(service => service.RegisterAsync(It.IsAny<CredentialsModel>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Register(credentials);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Utilisateur enregistré avec succès", okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithToken_WhenCredentialsAreValid()
    {
        // Arrange
        var credentials = new CredentialsModel
        {
            Mail = "test@mail.com",
            Password = "password123"
        };

        var token = "valid-jwt-token";
        _mockCredentialService.Setup(service => service.LoginAsync(credentials)).ReturnsAsync(token);

        // Act
        var result = await _controller.Login(credentials);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<dynamic>(okResult.Value);
        Assert.Equal(token, returnValue.Token);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorizedResult_WhenCredentialsAreInvalid()
    {
        // Arrange
        var credentials = new CredentialsModel
        {
            Mail = "wrong@mail.com",
            Password = "wrongpassword"
        };

        _mockCredentialService.Setup(service => service.LoginAsync(credentials)).ReturnsAsync((string)null);

        // Act
        var result = await _controller.Login(credentials);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Identifiants invalides", unauthorizedResult.Value);
    }
}
