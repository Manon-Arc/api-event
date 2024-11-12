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
    private readonly PermissionService _permissionService;

    public UserController (UsersService usersService, PermissionService permissionService)
    {
        _usersService = usersService;
        _permissionService = permissionService;
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
    public async Task<IActionResult> PostUser([FromQuery] CreateUserDto userDto)
    {
        if (!_usersService.IsEmailValid(userDto.Mail))
        {
            return BadRequest(new { Message = "Invalid email address format." });
        }
        
        var existingUser = await _usersService.GetByEmailAsync(userDto.Mail);
        if (existingUser != null)
        {
            return Conflict(new { Message = "Email is already in use." });
        }
        var newUser = new UserModel
            
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Mail = userDto.Mail,
        };
        
        await _usersService.CreateAsync(newUser);
        await _permissionService.CreateAsync(newUser.Id);
        
        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id}")]
    public async void PutUser(string id, [FromQuery] UserModel userModel)
    {
        await _usersService.UpdateAsync(id, userModel);
    }
    
    [HttpDelete("{id}")]
    public async void DeleteEvent(string id)
    {
        await _usersService.RemoveAsync(id);
    }
}

