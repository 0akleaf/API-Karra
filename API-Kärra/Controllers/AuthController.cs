using Microsoft.AspNetCore.Mvc;
using APIKarra.Services;
using APIKarra.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace APIKarra.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var result = await _authService.RegisterAsync(model);
        return result.Succeeded ? Ok("User created") : BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var token = await _authService.LoginAsync(model);
        return token != null ? Ok(new { token }) : Unauthorized("Invalid credentials");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var result = await _authService.RequestPasswordResetAsync(model.Email);

        return result ? Ok("Password reset link sent") : NotFound("User not found");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var result = await _authService.ResetPasswordAsync(model);
        return result.Succeeded ? Ok("Password has been reset.") : BadRequest(result.Errors);
    }

    // [Authorize(Roles = "ADMIN")]
    [HttpPost("changerole")]
    public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRolesDto model)
    {
        var result = await _authService.ChangeUserRoleAsync(model);
        return result.Succeeded ? Ok("Role updated") : BadRequest(result.Errors);
    }

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequestDto model)
    {
        var user = await _authService.FindUserByUsernameAsync(model.Username);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var token = await _authService.GenerateJwtTokenAsync(user);
        return Ok(new { token });
    }

}

