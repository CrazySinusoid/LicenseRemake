using LicenseRemake.Application.Interfaces;
using LicenseRemake.DTO.AdminPanel;
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
    public async Task<IActionResult> CreateAdmin(CancellationToken ct)
    {
        await _service.CreateAdminAsync(ct);
        return Ok();
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        await _service.CreateUserAsync(
            request.UserName,
            request.Password,
            request.UserTypeId,
            ct);

        return Ok();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await _service.ChangePasswordAsync(request.UserId, request.NewPassword, ct);
        return Ok();
    }

    [HttpPost("block")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BlockUser([FromBody] ChangeBlockStatusRequest request, CancellationToken ct)
    {
        await _service.ChangeBlockStatusAsync(request.UserId, request.IsActive, ct);
        return Ok();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromQuery] Guid guid, CancellationToken ct)
    {
        await _service.DeleteUserAsync(guid, ct);
        return Ok();
    }

    [HttpGet("list")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var users = await _service.GetAllAsync(ct);
        return Ok(users);
    }
}