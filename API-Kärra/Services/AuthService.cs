using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIKarra.Models;
using APIKarra.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;

namespace APIKarra.Services;
public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpClient _httpClient;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator,
        HttpClient httpClient)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
        _httpClient = httpClient;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto model)
    {
        var user = new User
        {
            UserName = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Lägg till en standardroll om du vill
            await _userManager.AddToRoleAsync(user, "USER");
        }

        return result;
    }

    public async Task<string?> LoginAsync(LoginDto model)
    {
        var user = await FindUserByUsernameAsync(model.Username);
        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return null;

        // Use the extracted method instead of duplicating token generation
        return await GenerateJwtTokenAsync(user);
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetUrl = _linkGenerator.GetUriByAction(
            _httpContextAccessor.HttpContext,
            action: "ResetPassword",
            controller: "Auth",
            values: new { token, email = user.Email }
        );

        // Skicka datan till Logic App
        var payload = new
        {
            email = user.Email,
            name = user.UserName,
            resetLink = resetUrl
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var logicAppUrl = _config["LogicApp:ResetEmailUrl"]; // lägg in URL i appsettings.json
        var response = await _httpClient.PostAsync(logicAppUrl, content);

        return response.IsSuccessStatusCode;
    }


    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return IdentityResult.Failed();

        return await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
    }

    public async Task<IdentityResult> ChangeUserRoleAsync(ChangeUserRolesDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return removeResult;

        var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
        return addResult;
    }

    public async Task<User?> FindUserByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id), // Add user ID for better identification
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Create signing credentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create and return token
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<IdentityResult> ChangePasswordAsync(PasswordChangeDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
    }
}
