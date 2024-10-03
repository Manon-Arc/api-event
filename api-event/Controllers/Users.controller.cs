using Microsoft.AspNetCore.Mvc;

namespace api_event.Controllers;

[Route("/[controller]")]
[ApiController]
public class Users_controller : ControllerBase
{
    private readonly UsersContext _context;

    public Users_controller(UsersContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsersModel>>> GetUsers()
    {
        return await _context.Users.Select(user => UserToModel(user)).ToListAsync();
    }
}