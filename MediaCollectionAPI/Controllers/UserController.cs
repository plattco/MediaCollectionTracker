// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using MediaCollectionAPI.Data;
using MediaCollectionAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly MediaCollectionContext _context;

    public UsersController(MediaCollectionContext context)
    {
        _context = context;
    }

    [HttpGet] 
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users.Select(u => new UserDto { Id = u.Id, Username = u.Username }).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username))
            return BadRequest("Username is required.");

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest("Username is already taken.");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = new byte[0],
            PasswordSalt = new byte[0]
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, new { userId = user.Id });
    }
}