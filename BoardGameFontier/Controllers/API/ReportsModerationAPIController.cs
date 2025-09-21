using BoardGameFontier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    [ApiController]
    [Route("api/reports/moderation")]
    public class ReportsModerationAPIController : ControllerBase
    {
        private readonly IModerationService _moderation;

        public class ModerationNote { public string? Notes { get; set; } }

        public ReportsModerationAPIController(IModerationService moderation)
        {
            _moderation = moderation;
        }

        // 審核通過
        [HttpPost("{id}/approve")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ModerationNote body)
        {
            await _moderation.ApproveReportAsync(id, body?.Notes);
            return Ok(new { success = true });
        }

        // 審核退回
        [HttpPost("{id}/reject")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id, [FromBody] ModerationNote body)
        {
            await _moderation.RejectReportAsync(id, body?.Notes);
            return Ok(new { success = true });
        }
    }
}
