using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Helpers;
using ProductManagement.Services;

namespace ProductManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, JwtTokenGenerator jwtTokenGenerator)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login request)
    {
        var user = await userService.ValidateCredentials(request);

        if (user == null)
            return Unauthorized();

        var token = jwtTokenGenerator.GenerateToken(user);

        var value = new AuthResponse
        {
            Token = token,
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email
        };
        return Ok(value);
    }
    
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized("Invalid token");

        if (await userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword))
            return Ok(new
            {
                message = "Successfully!"
            });

        return BadRequest("Invalid credentials");
    }
}
