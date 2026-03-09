using LicenseRemake.Application.Interfaces;
using LicenseRemake.DTO.Licensing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseRemake.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // доступ только через JWT (глобал сервис)
public class LicensingController : ControllerBase
{
    private readonly ILicensingService _licensingService;

    public LicensingController(ILicensingService licensingService)
    {
        _licensingService = licensingService;
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LicenseResponse>> Refresh(
        [FromBody] LicenseRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SerialNumber))
            return BadRequest("SerialNumber is required");

        var hostIp = GetClientIp();

        var result = await _licensingService.RefreshLicenseAsync(
            request.SerialNumber,
            hostIp,
            cancellationToken);

        return Ok(result);
    }

    private string GetClientIp()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.FirstOrDefault() ?? "unknown";

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
