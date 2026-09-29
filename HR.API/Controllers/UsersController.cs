using HR.Application.Common.Security;
using HR.Application.DTOs.Users;
using HR.Infrastructure.Identity;
using HR.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AppPolicies.AdminOnly)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserDto dto,
        CancellationToken cancellationToken)
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

        if (dto.EmployeeId is long employeeId)
        {
            var employeeExists = await _dbContext.Employees
                .AnyAsync(employee => employee.Id == employeeId, cancellationToken);

            if (!employeeExists)
            {
                return NotFound(new
                {
                    message = $"Employee with ID {employeeId} was not found."
                });
            }

            var employeeAlreadyLinked = await _userManager.Users
                .AnyAsync(user => user.EmployeeId == employeeId, cancellationToken);

            if (employeeAlreadyLinked)
            {
                return Conflict(new
                {
                    message = $"Employee with ID {employeeId} is already linked to a user."
                });
            }
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            EmployeeId = dto.EmployeeId
        };

        IdentityResult result;

        try
        {
            result = await _userManager.CreateAsync(
                user,
                dto.Password);
        }
        catch (DbUpdateException) when (dto.EmployeeId is long linkedEmployeeId)
        {
            if (!await _dbContext.Employees
                    .AnyAsync(employee => employee.Id == linkedEmployeeId, cancellationToken))
            {
                return NotFound(new
                {
                    message = $"Employee with ID {linkedEmployeeId} was not found."
                });
            }

            if (await _userManager.Users
                    .AnyAsync(existing => existing.EmployeeId == linkedEmployeeId, cancellationToken))
            {
                return Conflict(new
                {
                    message = $"Employee with ID {linkedEmployeeId} is already linked to a user."
                });
            }

            throw;
        }

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
            user.EmployeeId,
            Role = dto.Role
        });
    }
}
