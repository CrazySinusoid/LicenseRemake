using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseRemake.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "OK",
            UtcNow = DateTime.UtcNow
        });
    }

    [HttpGet("secure")]
    [Authorize]
    public IActionResult Secure()
    {
        return Ok(new
        {
            Status = "Authorized",
            User = User.Identity?.Name,
            UtcNow = DateTime.UtcNow
        });
    }
}
