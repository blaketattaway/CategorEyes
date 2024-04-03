using Microsoft.AspNetCore.Mvc;
using OneCore.CategorEyes.Business.Report;
using OneCore.CategorEyes.Commons.Requests;

namespace OneCore.CategorEyes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportBusiness _reportBusiness;

        public ReportController(IReportBusiness reportBusiness)
        {
            _reportBusiness = reportBusiness;
        }

        [HttpPost("Generate")]
        public async Task<IActionResult> Generate(ReportRequest request)
        {
            return Ok(await _reportBusiness.GenerateReport(request));
        }
    }
}
