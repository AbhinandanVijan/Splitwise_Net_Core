using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Splitwise.Api.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet] public IActionResult Get() => Ok(new { status = "ok" });
    }
}
