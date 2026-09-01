using AutoMapper;
using LMS.Api.DTOs.Users;
using LMS.Api.Models;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

/// <summary>
/// Handles administration of LMS users.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Teacher")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A list of users.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        List<User> users = await _userService.GetAllAsync();

        IEnumerable<UserDto> result = _mapper.Map<IEnumerable<UserDto>>(users);

        return Ok(result);
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The requested user.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] Guid id)
    {
        User? user = await _userService.GetUserByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">User information.</param>
    /// <returns>The newly created user.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto request)
    {
        User user = _mapper.Map<User>(request);

        user.Id = Guid.NewGuid();

        IdentityResult result = await _userService.CreateUserAsync(
            user,
            request.Password,
            request.Role
        );

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            _mapper.Map<UserDto>(user)
        );
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">Updated user information.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] Guid id,
        [FromBody] UserUpdateDto request)
    {
        IdentityResult result = await _userService.UpdateUserAsync(id, request);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(error => error.Code == "UserNotFound"))
            {
                return NotFound();
            }

            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Updates the status of a user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">The new user status.</param>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UserStatusUpdateDto request)
    {
        IdentityResult result = await _userService.UpdateUserStatusAsync(
            id,
            request.Status
        );

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}
