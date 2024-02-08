using CoreCommon.Infrastructure.Helpers;
using CoreCommon.Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace CoreCommon.Application.WebAPIBase.Controllers
{
    [Route("Debug")]
    [ApiController]
    public class CommonDebugController : BaseController
    {
        public IConfiguration Configuration { get; set; }

        public ILogService LogService { get; set; }

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

        [HttpGet("logtest")]
        public IActionResult LogTest()
        {
            foreach (var loglevel in Enum.GetValues<LogLevel>())
            {
                LogService.Log(loglevel, $"This is a {loglevel} message {DateTime.UtcNow}");
            }

            return SuccessResponse("Done");
        }

        [HttpGet("test/get")]
        public async Task<IActionResult> TestGet()
        {
            return await TestResponse();
        }

        [HttpPost("test/post")]
        public async Task<IActionResult> TestPost()
        {
            return await TestResponse();
        }

        private async Task<IActionResult> TestResponse()
        {
            if (Request.Query.ContainsKey("error"))
            {
                return ErrorResponse(message: Request.Query["error"]);
            }

            return SuccessResponse(await StreamHelper.ToStringAsync(Request.Body));
        }
    }
}
