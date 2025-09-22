using Microsoft.AspNetCore.Mvc;
using MediaCollectionAPI.Data;
using MediaCollectionAPI.Models;
using System.Security.Cryptography;
using System.Text;
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
    
    HttpPost("register")]
    public async Task<IActionResult> Register(string username, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
            return BadRequest("Username is already taken.");

        CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }
    
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}