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

        return Ok(new { Token = token });
    }
}
