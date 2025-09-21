using BoardGameFontier.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsApiController : ControllerBase
    {
        private readonly IAnalyticsService _service;
        private readonly ILogger<AnalyticsApiController> _logger;

        public AnalyticsApiController(IAnalyticsService service, ILogger<AnalyticsApiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            try
            {
                var data = _service.GetSummary(); // 保留空白邏輯
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest("[summary] 發生錯誤");
            }
        }

        [HttpGet("post-trend")]
        public IActionResult GetPostTrend()
        {
            try
            {
                var data = _service.GetPostTrend();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest("[post-trend] 發生錯誤");
            }
        }

        [HttpGet("category-distribution")]
        public IActionResult GetCategoryDistribution()
        {
            try
            {
                var data = _service.GetCategoryDistribution();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest("[category-distribution] 發生錯誤");
            }
        }
    }
}
