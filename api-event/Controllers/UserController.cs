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
    public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
    {
        List<UserModel> data = await _usersService.GetAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUser(String id)
    {
        UserModel? data = await _usersService.GetAsync(id);
        return Ok(data);
    }

    [HttpPost]
    public async void PostUser([FromQuery] UserModel userModel)
    {
        await _usersService.CreateAsync(userModel);
    }

    [HttpPut("{id}")]
    public async void PutUser(string id, [FromBody] UserModel userModel)
    {
        await _usersService.UpdateAsync(id, userModel);
    }
}