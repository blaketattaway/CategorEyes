using Microsoft.AspNetCore.Mvc;
using OneCore.CategorEyes.Business.Log;
using OneCore.CategorEyes.Commons.Requests;

namespace OneCore.CategorEyes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogBusiness _logBusiness;

        public LogController(ILogBusiness logBusiness)
        {
            _logBusiness = logBusiness;
        }

        [HttpPost("GetLogs")]
        public async Task<IActionResult> GetLogs(LogRequest request)
        {
            return Ok(await _logBusiness.GetPaged(request));
        }

        [HttpPost("AddInteraction")]
        public async Task<IActionResult> AddInteraction(UserInteractionRequest request)
        {
            await _logBusiness.AddUserInteraction(request);
            return Ok(new { });
        }
    }
}
