using Microsoft.AspNetCore.Mvc;
using ProductManagement.Helpers;
using ProductManagement.Services;

namespace ProductManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, JwtTokenGenerator jwtTokenGenerator)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userService.FindByUsernameAsync(request.Username);
        if (user == null || !await userService.CheckPasswordAsync(user, request.Password))
            return Unauthorized();

        var token = jwtTokenGenerator.GenerateToken(user);
        return Ok(new { Token = token });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}