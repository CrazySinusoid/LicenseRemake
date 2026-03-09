using LicenseRemake.Application.Interfaces;
using LicenseRemake.DTO.Licensing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseRemake.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public class LicenseLogController : ControllerBase
{
    private readonly ILicenseLogService _service;

    public LicenseLogController(ILicenseLogService service)
    {
        _service = service;
    }

    [HttpPost("search")]
    public async Task<ActionResult<LicenseLogSearchResult>> Search(
        [FromBody] LicenseLogSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SearchAsync(request, cancellationToken);
        return Ok(result);
    }
}
