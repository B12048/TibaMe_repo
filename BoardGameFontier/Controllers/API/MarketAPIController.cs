using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BoardGameFontier.Controllers
{
    [ApiController]
    [Route("api/market")]
    public class MarketApiController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<MarketApiController> _logger;

        public MarketApiController(IProductService service, ILogger<MarketApiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("loadMarketData")]
        public IActionResult loadMarketData([FromBody] LoadMarketDataDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var data = _service.GetAllProductByUser(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[loadMarketData] Error: {ex}");
                return BadRequest("ERROR");
            }
        }
    }
}
