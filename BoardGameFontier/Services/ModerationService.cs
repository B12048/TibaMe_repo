using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BoardGameFontier.Repostiory;                 // ApplicationDbContext
using BoardGameFontier.Repostiory.Entity;         // Report, Post, Comment, PrivateMessages

namespace BoardGameFontier.Services
{
    public class ModerationService : IModerationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAdminService _adminService;   // 會員管理（鎖帳）
        private readonly ICurrentUserService _current;  // 目前審核者（用於私訊 SenderId）
        private readonly ILogger<ModerationService> _logger;

        private const int WarningThreshold = 5;

        public ModerationService(
            ApplicationDbContext db,
            IAdminService adminService,
            ICurrentUserService current,
            ILogger<ModerationService> logger)
        {
            _db = db;
            _adminService = adminService;
            _current = current;
            _logger = logger;
        }

        public async Task ApproveReportAsync(int reportId, string? notes)
        {
            var report = await _db.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
            if (report == null) throw new ArgumentException("Report not found.");

            // 已處理就不重覆
            if (string.Equals(report.Status, "Resolved", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(report.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Report {ReportId} already handled with status {Status}", report.Id, report.Status);
                return;
            }

            // 1) 找被檢舉者與標題（找不到不丟例外）
            (string targetUserId, string title) = await ResolveTargetAsync(report);

            // 2) 先標記通過，並把被檢舉者寫入 Report（之後計數都靠 Reports 表）
            report.Status = "Resolved";                 // Pending / Resolved / Rejected
            report.ResolutionNotes = notes;
            report.ResolvedAt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(targetUserId))
                report.TargetUserId = targetUserId;

            _db.Reports.Update(report);
            await _db.SaveChangesAsync();

            // 3) 刪違規內容（Post/Comment）
            bool removed = await DeleteReportedContentAsync(report);

            // 4) 站內私訊通知
            var msgToReporter = removed
                ? $"您對「{title}」的檢舉已審核通過，系統已移除違規內容並記一支警告。"
                : $"您對「{title}」的檢舉已審核通過，系統已記錄一次警告。";
            await NotifyReporterAsync(report.ReporterId, msgToReporter);

            if (!string.IsNullOrWhiteSpace(targetUserId))
            {
                var msgToTarget = removed
                    ? $"提醒：您在「{title}」的內容經審核確認違規，系統已記錄一次警告，且該內容已被移除。"
                    : $"提醒：您在「{title}」的內容經審核確認違規，系統已記錄一次警告。";
                await NotifyTargetAsync(targetUserId, msgToTarget);

                // 5) 直接用 Reports 表計數（與內容是否還存在無關）
                var count = await _db.Reports
                    .Where(r => r.Status == "Resolved" && r.TargetUserId == targetUserId)
                    .CountAsync();

                // 6) 剛好第 5 次 → 鎖帳
                if (count == WarningThreshold)
                {
                    var ok = _adminService.UpdateUserLockout(targetUserId, false); // false = 鎖住
                    _logger.LogInformation("Auto-lock user {UserId} at {Count} warnings. Result={Ok}",
                        targetUserId, count, ok);
                }
            }
            else
            {
                _logger.LogInformation("Skip notify-target / counting: cannot infer target user for report {ReportId}.", report.Id);
            }
        }

        public async Task RejectReportAsync(int reportId, string? notes)
        {
            var report = await _db.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
            if (report == null) throw new ArgumentException("Report not found.");

            if (string.Equals(report.Status, "Resolved", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(report.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Report {ReportId} already handled with status {Status}", report.Id, report.Status);
                return;
            }

            report.Status = "Rejected";
            report.ResolutionNotes = notes;
            report.ResolvedAt = DateTime.Now;
            _db.Reports.Update(report);
            await _db.SaveChangesAsync();

            await NotifyReporterAsync(report.ReporterId, "您提交的檢舉經審核未通過（退回）。感謝您的協助。");
        }

        /// <summary>
        /// 推回被檢舉者的 UserId 與標題；找不到內容時不丟例外，回傳 ("", "貼文/留言")。
        /// 注意：Report.ReportedId 多為 string；此處轉 int 再查 Post/Comment。
        /// </summary>
        private async Task<(string targetUserId, string title)> ResolveTargetAsync(Report report)
        {
            var kind = (report.ReportedType ?? "").Trim();

            if (kind.Equals("Post", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(report.ReportedId, out var postId))
                {
                    _logger.LogWarning("ResolveTarget: invalid ReportedId for Post: {ReportedId}", report.ReportedId);
                    return ("", "貼文");
                }

                var r = await _db.Posts
                    .Where(p => p.Id == postId)
                    .Select(p => new { p.AuthorId, p.Title })
                    .FirstOrDefaultAsync();

                if (r == null)
                {
                    _logger.LogInformation("ResolveTarget: post {PostId} not found.", postId);
                    return ("", "貼文");
                }
                return (r.AuthorId, r.Title ?? "貼文");
            }

            if (kind.Equals("Comment", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(report.ReportedId, out var commentId))
                {
                    _logger.LogWarning("ResolveTarget: invalid ReportedId for Comment: {ReportedId}", report.ReportedId);
                    return ("", "留言");
                }

                var comment = await _db.Comments
                    .Where(c => c.Id == commentId)
                    .Select(c => new { c.AuthorId, c.PostId })
                    .FirstOrDefaultAsync();

                if (comment == null)
                {
                    _logger.LogInformation("ResolveTarget: comment {CommentId} not found.", commentId);
                    return ("", "留言");
                }

                var title = await _db.Posts
                    .Where(p => p.Id == comment.PostId)
                    .Select(p => p.Title)
                    .FirstOrDefaultAsync() ?? "留言";

                return (comment.AuthorId, title);
            }

            if (kind.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                // 若支援直接檢舉使用者：在此把 ReportedId 對到 Identity UserId
                return ("", "使用者");
            }

            _logger.LogWarning("ResolveTarget: unknown ReportedType {Type}", report.ReportedType);
            return ("", "內容");
        }

        /// <summary>
        /// 實際刪除違規內容（Post / Comment）。刪留言時同步維護貼文的 CommentCount。
        /// 刪除失敗不會中斷審核流程，僅記錄 Log。
        /// </summary>
        private async Task<bool> DeleteReportedContentAsync(Report report)
        {
            try
            {
                var kind = (report.ReportedType ?? "").Trim();

                if (kind.Equals("Post", StringComparison.OrdinalIgnoreCase))
                {
                    if (!int.TryParse(report.ReportedId, out var postId))
                    {
                        _logger.LogWarning("Delete post failed: invalid ReportedId: {ReportedId}", report.ReportedId);
                        return false;
                    }

                    var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                    if (post == null)
                    {
                        _logger.LogInformation("Delete post skipped: post {PostId} not found.", postId);
                        return false;
                    }

                    // 若未設定 FK cascade，可先刪關聯資料；此處示範刪留言
                    var relatedComments = _db.Comments.Where(c => c.PostId == postId);
                    _db.Comments.RemoveRange(relatedComments);

                    _db.Posts.Remove(post);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Deleted post {PostId} due to approved report {ReportId}", postId, report.Id);
                    return true;
                }

                if (kind.Equals("Comment", StringComparison.OrdinalIgnoreCase))
                {
                    if (!int.TryParse(report.ReportedId, out var commentId))
                    {
                        _logger.LogWarning("Delete comment failed: invalid ReportedId: {ReportedId}", report.ReportedId);
                        return false;
                    }

                    var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                    if (comment == null)
                    {
                        _logger.LogInformation("Delete comment skipped: comment {CommentId} not found.", commentId);
                        return false;
                    }

                    // 維護貼文留言數
                    var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == comment.PostId);
                    if (post != null && post.CommentCount > 0)
                    {
                        post.CommentCount--;
                        _db.Posts.Update(post);
                    }

                    _db.Comments.Remove(comment);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Deleted comment {CommentId} due to approved report {ReportId}", commentId, report.Id);
                    return true;
                }

                // 其他型別（User）先不處理
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteReportedContentAsync failed. ReportId={ReportId} Type={Type} ReportedId={ReportedId}",
                    report.Id, report.ReportedType, report.ReportedId);
                return false;
            }
        }

        // === 站內私訊：用目前審核者(_current.UserId) 當 SenderId，寫到 PrivateMessages ===
        private async Task NotifyReporterAsync(string reporterUserId, string message)
            => await SendPrivateMessageAsync(reporterUserId, message);

        private async Task NotifyTargetAsync(string targetUserId, string message)
            => await SendPrivateMessageAsync(targetUserId, message);

        private async Task SendPrivateMessageAsync(string toUserId, string text)
        {
            if (string.IsNullOrWhiteSpace(toUserId)) return;
            try
            {
                _db.Set<PrivateMessages>().Add(new PrivateMessages
                {
                    SenderId = _current.UserId, // 審核者（Admin）
                    ReceiverId = toUserId,
                    Message = text,
                    CreatedAt = DateTime.Now
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Send private message failed. to={To}, text={Text}", toUserId, text);
            }
        }
    }
}
