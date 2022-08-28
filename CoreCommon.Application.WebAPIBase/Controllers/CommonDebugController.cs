using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreCommon.Application.WebAPIBase.Controllers
{
    [Route("Debug")]
    [ApiController]
    public class CommonDebugController : BaseController
    {
        public IConfiguration Configuration { get; set; }

        [HttpGet("info")]
        public IActionResult Info()
        {
            return Json(new
            {
                SettingName = Configuration["SettingName"],
                ProjectName = Configuration["ProjectName"],
                Version = Configuration["Version"],
                Environment = Configuration["ASPNETCORE_ENVIRONMENT"],
            });
        }
    }
}
