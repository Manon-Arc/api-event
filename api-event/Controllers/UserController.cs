using api_event.Models;
using api_event.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class UserController(UsersService usersService, PermissionService permissionService)
    : ControllerBase
{
    /// <summary>
    ///     Retrieves all users.
    /// </summary>
    /// <remarks>This endpoint returns a list of all users in the system.</remarks>
    /// <returns>A list of <see cref="UserDto" /> objects.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// ///
    /// <response code="500">If there was an error retrieving the events.</response>
    /// <response code="500">If there was an error retrieving the events.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        try
        {
            var data = await usersService.GetAsync();
            if (data == null) return NotFound();
            return Ok(data);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving events." });
        }
    }

    /// <summary>
    ///     Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The <see cref="UserDto" /> with the specified ID.</returns>
    /// <response code="200">Returns the user with the specified ID.</response>
    /// <response code="404">If no user is found with the specified ID.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "User ID is required." });

        var data = await usersService.GetAsync(id);
        if (data == null) return NotFound(new { Message = "User not found." });
        return Ok(data);
    }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="userIdlessDto">The user data transfer object containing new user information.</param>
    /// <returns>The newly created user.</returns>
    /// <response code="201">Returns the newly created user.</response>
    /// <response code="400">If the email is invalid.</response>
    /// <response code="409">If the email is already in use.</response>
    [HttpPost]
    public async Task<IActionResult> PostUser([FromQuery] UserIdlessDto userIdlessDto)
    {
        if (!usersService.IsEmailValid(userIdlessDto.mail))
            return BadRequest(new { Message = "Invalid email address format." });

        var existingUser = await usersService.GetByEmailAsync(userIdlessDto.mail);
        if (existingUser != null) return Conflict(new { Message = "Email is already in use." });
        var newUser = new UserDto
        {
            firstName = userIdlessDto.firstName,
            lastName = userIdlessDto.lastName,
            mail = userIdlessDto.mail
        };

        await usersService.CreateAsync(newUser);
        await permissionService.CreateAsync(newUser.Id!);

        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }

    /// <summary>
    ///     Updates an existing user with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="userIdlessDto">The updated user data.</param>
    /// <response code="204">Return the newly updated user.</response>
    /// <response code="404">If no user is found with the specified ID.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromQuery] UserIdlessDto userIdlessDto)
    {
        var updatedUser = await usersService.UpdateAsync(id, userIdlessDto);
        if (updatedUser == null) return NotFound($"User with ID {id} not found.");

        return Ok(updatedUser); // Return the updated user
    }

    /// <summary>
    ///     Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <response code="204">If the user was successfully deleted.</response>
    /// <response code="404">If no user is found with the specified ID.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var existingUser = await usersService.GetAsync(id);
        if (existingUser == null) return NotFound(new { Message = "User not found." });

        await usersService.RemoveAsync(id);
        return NoContent();
    }
}