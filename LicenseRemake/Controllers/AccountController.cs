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
    public async Task<ActionResult<OperationResult>> CreateAdmin(CancellationToken ct)
    {
        await _service.CreateAdminAsync(ct);
        return Ok(new OperationResult(true));
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
    public async Task<ActionResult<OperationResult>> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await _service.ChangePasswordAsync(request.UserId, request.NewPassword, ct);
        return Ok(new OperationResult(true));
    }

    [HttpPost("block")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OperationResult>> BlockUser([FromBody] ChangeBlockStatusRequest request, CancellationToken ct)
    {
        await _service.ChangeBlockStatusAsync(request.UserId, request.IsActive, ct);
        return Ok(new OperationResult(true));
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OperationResult>> Delete([FromQuery] Guid guid, CancellationToken ct)
    {
        await _service.DeleteUserAsync(guid, ct);
        return Ok(new OperationResult(true));
    }

    [HttpGet("list")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserListItemDto>>> List(CancellationToken ct)
    {
        var users = await _service.GetAllAsync(ct);
        return Ok(users);
    }
}