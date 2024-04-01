using Microsoft.AspNetCore.Mvc;
using OneCore.CategorEyes.Business.Analysis;
using OneCore.CategorEyes.Commons.Requests;

namespace OneCore.CategorEyes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisBusiness _analysisBusiness;

        public AnalysisController(IAnalysisBusiness analysisBusiness)
        {
            _analysisBusiness = analysisBusiness;
        }

        [HttpPost("Analyze")]
        public async Task<IActionResult> Analyze(AnalysisRequest request)
        {
            return Ok(await _analysisBusiness.Analyze(request));
        }
    }
}
