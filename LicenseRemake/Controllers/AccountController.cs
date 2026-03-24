using LicenseRemake.Application.Interfaces;
using LicenseRemake.DTO.AdminPanel;
using LicenseRemake.DTO.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    [HttpPost("create-admin")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiErrorResponse>> CreateAdmin(CancellationToken ct)
    {
        await _service.CreateAdminAsync(ct);

        return Ok(new ApiErrorResponse
        {
            err_code = 0,
            err_code_string = "no_error",
            time_stamp = DateTime.UtcNow
        });
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var id = await _service.CreateUserAsync(
            request.UserName,
            request.Password,
            request.UserTypeId,
            ct);

        return Ok(new CreateUserResponse(id));
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<UserDto>> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var user = await _service.ChangePasswordAsync(request.UserId, request.NewPassword, ct);
        return Ok(user);
    }

    [HttpPost("block")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> BlockUser([FromBody] ChangeBlockStatusRequest request, CancellationToken ct)
    {
        var user = await _service.ChangeBlockStatusAsync(request.UserId, request.IsActive, ct);
        return Ok(user);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> Delete([FromQuery] Guid guid, CancellationToken ct)
    {
        var user = await _service.DeleteUserAsync(guid, ct);
        return Ok(user);
    }

    [HttpGet("list")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> List(CancellationToken ct)
    {
        var users = await _service.GetAllAsync(ct);
        return Ok(users);
    }
}
