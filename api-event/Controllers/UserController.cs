using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

//[Authorize]
[Route("/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UsersService _usersService;

    public UserController ( UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        List<User> data = await _usersService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(String id)
    {
        User? data = await _usersService.GetAsync(id);
        return Ok(data);
    }

    [HttpPost]
    public async void PostUser([FromQuery] User user)
    {
        await _usersService.CreateAsync(user);
    }

    [HttpPut("{id}")]
    public async void PutUser(string id, [FromBody] User user)
    {
        await _usersService.UpdateAsync(id, user);
    }
}