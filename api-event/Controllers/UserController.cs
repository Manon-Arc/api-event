using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UsersService _usersService;
    static String routeName = "users";

    public UserController ( UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _usersService.GetAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(String id)
    {
        return (await _usersService.GetAsync(id))!;
    }
}