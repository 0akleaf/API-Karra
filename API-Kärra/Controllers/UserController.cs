using APIKarra.Data;
using APIKarra.Models;
using APIKarra.Services;
using APIKarra.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIKarra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly AuthService _authService;

        public UserController(UserService userService, UserManager<User> userManager, AuthService authService)
        {
            _userService = userService;
            _userManager = userManager;
            _authService = authService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // GET: api/User/search?email=example@domain.com
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUsers([FromQuery] string email)
        {
            var users = await _userService.GetByEmailAsync(email);
            return Ok(users);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] UserUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Convert DTO to user model
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }
                
                // Update properties from DTO
                user.UserName = updateDto.UserName;
                user.Email = updateDto.Email;
                user.FirstName = updateDto.FirstName;
                user.LastName = updateDto.LastName;
                user.Address = updateDto.Address;
                user.City = updateDto.City;
                user.ZIPCode = updateDto.ZipCode;
                user.EmailConfirmed = updateDto.EmailConfirmed;
                user.PhoneNumber = updateDto.PhoneNumber; // Add the missing phone number update
                
                // Use the service to update the user
                await _userService.UpdateAsync(id, user);
                
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            await _userService.AddAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Use the service to delete the user
                await _userService.DeleteAsync(id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/User/changerole
        [HttpPut("changerole")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRolesDto model)
        {
            try
            {
                var result = await _authService.ChangeUserRoleAsync(model);
                return result.Succeeded ? Ok("Role updated") : BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/User/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeDto model)
        {
            try
            {
                var result = await _authService.ChangePasswordAsync(model);
                return result.Succeeded 
                    ? Ok("Password successfully changed") 
                    : BadRequest("Failed to change password. Please verify your current password is correct.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<bool> UserExists(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user != null;
        }
    }
}