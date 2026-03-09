using LicenseRemake.Application.Interfaces;
using LicenseRemake.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _service;

    public TokenController(ITokenService service)
        => _service = service;

    [AllowAnonymous]
    [HttpPost("PasswordLogin")]
    public async Task<IActionResult> PasswordLogin(
        [FromBody] PasswordLoginRequest request,
        CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        return Ok(await _service.PasswordLoginAsync(request, ip, ct));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("emit-terminal-refreshtoken")]
    public async Task<IActionResult> EmitRefresh(
        [FromBody] EmitTerminalRefreshTokenRequest request,
        CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var token = await _service.EmitTerminalRefreshTokenAsync(request, ip, ct);
        return Ok(new { RefreshToken = token });
    }

    [AllowAnonymous]
    [HttpPost("RefreshTokenLogin")]
    public async Task<IActionResult> RefreshLogin(
        [FromBody] RefreshTokenLoginRequest request,
        CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        return Ok(await _service.RefreshTokenLoginAsync(request, ip, ct));
    }
}
