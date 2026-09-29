using HR.Application.Common.Security;
using HR.Application.DTOs.Users;
using HR.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!AppRoles.All.Contains(dto.Role))
        {
            return BadRequest(new
            {
                message = "Invalid role."
            });
        }

        var existingUser =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser is not null)
        {
            return Conflict(new
            {
                message = "User already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true
        };

        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors
                    .Select(x => x.Description)
            });
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                dto.Role);

        if (!roleResult.Succeeded)
        {
            var cleanupResult = await _userManager.DeleteAsync(user);

            if (!cleanupResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Role assignment failed and the newly created user could not be removed.",
                    roleErrors = roleResult.Errors
                        .Select(x => x.Description),
                    cleanupErrors = cleanupResult.Errors
                        .Select(x => x.Description)
                });
            }

            return BadRequest(new
            {
                message = "Role assignment failed. The newly created user was removed.",
                errors = roleResult.Errors
                    .Select(x => x.Description)
            });
        }

        return Created("", new
        {
            user.Id,
            user.Email,
            Role = dto.Role
        });
    }
}
