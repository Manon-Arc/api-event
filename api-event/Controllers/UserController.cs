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

    public UserController(UsersService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>
    ///     Get all users
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
    {
        var data = await _usersService.GetAsync();
        return Ok(data);
    }

    /// <summary>
    ///     Get user with specified identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUser(string id)
    {
        var data = await _usersService.GetAsync(id);
        return Ok(data);
    }

    /// <summary>
    ///     Create new user
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> PostUser([FromQuery] CreateUserDto userDto)
    {
        var newUser = new UserModel
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Mail = userDto.Mail,
            Permission = 1
        };

        await _usersService.CreateAsync(newUser);
        return Ok(newUser);
    }

    /// <summary>
    ///     Replace data of user with specified identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async void PutUser(string id, [FromQuery] UserModel userModel)
    {
        await _usersService.UpdateAsync(id, userModel);
    }

    /// <summary>
    ///     Delete user which has specified identifier
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async void DeleteEvent(string id)
    {
        await _usersService.RemoveAsync(id);
    }
}