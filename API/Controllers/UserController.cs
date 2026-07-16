using WorkoutTrackerApi.Infrastructure;
using WorkoutTrackerApi.Domain;
using WorkoutTrackerApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WorkoutTrackerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase {
    private readonly UserService _userService;
    private readonly JwtService _jwtService;
    private readonly PasswordHasher _passwordHasher;
    private readonly WorkoutTrackerApiDbContext _context;
    public UserController(UserService userService, JwtService jwtService, PasswordHasher passwordHasher, WorkoutTrackerApiDbContext context) {
        _userService = userService;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _context = context;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> RegisterUser(RegisterUserDto dto) {
        var result = await _userService.RegisterUser(dto);
        
        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser(LoginUserDto dto) {
        var theUser = await _context.UserTable
            .SingleOrDefaultAsync(x => x.Email == dto.Email);

        if (theUser == null || !_passwordHasher.VerifyPassword(dto.Password, theUser.PasswordHashed)) 
            return Unauthorized();
        
        var accessToken = _jwtService.Generate_JWT(theUser);
        var refreshToken = _jwtService.Generate_RefreshToken(theUser.Id.ToString());

        _context.RefreshTokenTable.Add(refreshToken);
        await _context.SaveChangesAsync();

        Response.Cookies.Append("RefreshToken", refreshToken.Token, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshToken.ExpiresAt
        });

        return Ok(new {Token = accessToken});
    
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh() {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var valueOfRefreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(valueOfRefreshToken))
            return Unauthorized();

        var kyuuRefreshToken = await _context.RefreshTokenTable.SingleOrDefaultAsync(n => n.Token == valueOfRefreshToken);

        if (kyuuRefreshToken == null || !kyuuRefreshToken.IsActive)
            return Unauthorized();

        kyuuRefreshToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = _jwtService.Generate_RefreshToken(kyuuRefreshToken.UserId);
        _context.RefreshTokenTable.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var um = await _context.UserTable.SingleOrDefaultAsync(n => n.Id == int.Parse(kyuuRefreshToken.UserId));
        var newAccessToken = _jwtService.Generate_JWT(um);

        Response.Cookies.Append("RefreshToken", newRefreshToken.Token, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = newRefreshToken.ExpiresAt
        });

        return Ok(new { Token = newAccessToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout() {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var valueOfRefreshToken = Request.Cookies["RefreshToken"];

        if (!string.IsNullOrEmpty(valueOfRefreshToken)) {
            var kyuuRefreshToken = await _context.RefreshTokenTable.SingleOrDefaultAsync(n => n.Token == valueOfRefreshToken);
            if (kyuuRefreshToken != null) {
                kyuuRefreshToken.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        Response.Cookies.Delete("RefreshToken");
        return NoContent();
    }
}