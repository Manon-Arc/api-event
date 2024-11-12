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
    public async Task<ActionResult> PostUser([FromQuery] CreateUserDto userDto)
    {
        var newUser = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Mail = userDto.Mail,
            Permission = 1
        };

        await _usersService.CreateAsync(newUser);
        return Ok(newUser);
    }

    [HttpPut("{id}")]
    public async void PutUser(string id, [FromBody] UserModel userModel)
    {
        await _usersService.UpdateAsync(id, userModel);
    }
}

