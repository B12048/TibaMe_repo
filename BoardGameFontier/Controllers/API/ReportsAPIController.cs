using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameFontier.Controllers.API
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsAPIController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _context;

        public ReportsAPIController(
            IReportService reportService,
            ICurrentUserService currentUserService,
            ApplicationDbContext context)
        {
            _reportService = reportService;
            _currentUserService = currentUserService;
            _context = context;
        }
        [HttpGet("unreviewed")]
        public IActionResult GetUnreviewedReports()
        {
            var reports = _reportService.GetUnreviewedReports();
            return Ok(reports);
        }

        [HttpPost("updateStatus")]
        public IActionResult UpdateStatus(int id, [FromQuery] string status, [FromQuery] string? notes = null)
        {
            var result = _reportService.UpdateReportStatus(id, status, notes);
            if (!result) return NotFound(new { success = false, message = "找不到檢舉" });
            return Ok(new { success = true });
        }

        [HttpGet("reviewed")]
        public IActionResult GetReviewedReports()
        {
            var reports = _reportService.GetReports(null, 0, 100)
                .Where(r => r.Status == "Resolved" || r.Status == "Rejected")
                .ToList();

            return Ok(reports);
        }
        [HttpPost("createReport")]
        public IActionResult CreateReport([FromBody] ReportRequest request)
        {
            if (string.IsNullOrEmpty(_currentUserService.UserId))
                return Unauthorized(new { success = false, message = "請先登入" });

            var result = _reportService.CreateReport(
                request.ReportedType,
                request.ReportedId,
                request.Reason,
                request.Description
            );

            if (!result)
                return BadRequest(new { success = false, message = "檢舉失敗" });

            // 取出最新的這筆檢舉紀錄
            var report = _context.Reports
                .OrderByDescending(r => r.Id)
                .FirstOrDefault(r => r.ReporterId == _currentUserService.UserId
                                     && r.ReportedType == request.ReportedType
                                     && r.ReportedId == request.ReportedId.ToString());

            if (report == null)
                return Ok(new { success = true, message = "檢舉成功" });

            // 用 ReportService 補齊 DTO

            var dto = ReportService.BuildReportDto(_context, report);


            return Ok(new
            {
                success = true,
                message = "檢舉成功",
                data = dto
            });
        }
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? searchText, [FromQuery] int skip = 0, [FromQuery] int take = 100)
        {
            var reports = _reportService.GetReports(searchText, skip, take);
            return Ok(reports);
        }

    }
}

    
