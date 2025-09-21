using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameFontier.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ReportService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        // 新增檢舉
        public bool CreateReport(string reportedType, int reportedId, string reason, string? description)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return false; // 使用者未登入

            var taiwanNow = DateTime.UtcNow.AddHours(8);

            var report = new Report
            {
                ReporterId = userId,
                ReportedType = reportedType,
                ReportedId = reportedId.ToString(),
                Reason = reason,
                Description = description,
                Status = "Pending",
                CreatedAt = taiwanNow
            };

            _context.Reports.Add(report);
            _context.SaveChanges();

            return true;
        }


        public static ReportDto BuildReportDto(ApplicationDbContext context, Report r)
        {
            string? reportedTitle = null;
            string? reportedContent = null;
            string? reportedUrl = null;
            bool isHtml = false;

            if (r.ReportedType == "Post")
            {
                if (int.TryParse(r.ReportedId, out var pid))
                {
                    var post = context.Posts.FirstOrDefault(p => p.Id == pid);
                    if (post != null)
                    {
                        // 標題常見就叫 Title；如果你的欄位不是 Title，請一樣套用 TryGetPostContent 的方式做容錯
                        var titleProp = post.GetType().GetProperty("Title");
                        reportedTitle = titleProp?.GetValue(post) as string ?? "貼文";

                        (reportedContent, isHtml) = TryGetPostContent(post);
                        reportedUrl = $"/Posts/Detail/{pid}";
                    }
                    else
                    {
                        reportedTitle = "[已刪除]";
                    }
                }
                else
                {
                    reportedTitle = "[已刪除]";
                }
            }
            else if (r.ReportedType == "Comment")
            {
                if (int.TryParse(r.ReportedId, out var cid))
                {
                    var comment = context.Comments.FirstOrDefault(c => c.Id == cid);
                    if (comment != null)
                    {
                        reportedTitle = "留言";
                        reportedContent = comment.Content;
                        isHtml = false;
                        reportedUrl = $"/Comments/Detail/{cid}";
                    }
                    else
                    {
                        reportedTitle = "[已刪除]";
                    }
                }
                else
                {
                    reportedTitle = "[已刪除]";
                }
            }

            return new ReportDto
            {
                Id = r.Id,
                ReporterId = r.ReporterId,
                ReportedType = r.ReportedType,
                ReportedId = int.TryParse(r.ReportedId, out var rid) ? rid : 0,
                Reason = r.Reason,
                Description = r.Description,
                Status = r.Status,
                ResolutionNotes = r.ResolutionNotes,
                ResolvedAt = r.ResolvedAt,
                CreatedAt = r.CreatedAt,

                ReportedTitle = reportedTitle ?? "[已刪除]",
                ReportedContent = string.IsNullOrWhiteSpace(reportedContent) ? null : reportedContent,
                ReportedContentIsHtml = isHtml,
                ReportedUrl = reportedUrl
            };
        }




        // 查詢檢舉 (支援搜尋 + 分頁)
        public List<ReportDto> GetUnreviewedReports()
        {
            var pending = _context.Reports
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var postIds = pending
                .Where(r => r.ReportedType == "Post")
                .Select(r => int.TryParse(r.ReportedId, out var id) ? id : (int?)null)
                .Where(id => id.HasValue).Select(id => id!.Value).ToList();

            var commentIds = pending
                .Where(r => r.ReportedType == "Comment")
                .Select(r => int.TryParse(r.ReportedId, out var id) ? id : (int?)null)
                .Where(id => id.HasValue).Select(id => id!.Value).ToList();

            var posts = _context.Posts.Where(p => postIds.Contains(p.Id)).ToDictionary(p => p.Id);
            var comments = _context.Comments.Where(c => commentIds.Contains(c.Id)).ToDictionary(c => c.Id);

            var list = new List<ReportDto>();
            foreach (var r in pending)
            {
                string? title = null;
                string? content = null;
                bool isHtml = false;
                string? url = null;

                if (r.ReportedType == "Post" && int.TryParse(r.ReportedId, out var pid))
                {
                    if (posts.TryGetValue(pid, out var p) && p != null)
                    {
                        title = (p.GetType().GetProperty("Title")?.GetValue(p) as string) ?? "貼文";
                        (content, isHtml) = TryGetPostContent(p);
                        url = $"/Posts/Detail/{pid}";
                    }
                    else title = "[已刪除]";
                }
                else if (r.ReportedType == "Comment" && int.TryParse(r.ReportedId, out var cid))
                {
                    if (comments.TryGetValue(cid, out var c) && c != null)
                    {
                        title = "留言";
                        content = c.Content;
                        isHtml = false;
                        url = $"/Comments/Detail/{cid}";
                    }
                    else title = "[已刪除]";
                }

                list.Add(new ReportDto
                {
                    Id = r.Id,
                    ReporterId = r.ReporterId,
                    ReportedType = r.ReportedType,
                    ReportedId = int.TryParse(r.ReportedId, out var rid) ? rid : 0,
                    Reason = r.Reason,
                    Description = r.Description,
                    Status = r.Status,
                    ResolutionNotes = r.ResolutionNotes,
                    ResolvedAt = r.ResolvedAt,
                    CreatedAt = r.CreatedAt,

                    ReportedTitle = title ?? "[已刪除]",
                    ReportedContent = string.IsNullOrWhiteSpace(content) ? null : content,
                    ReportedContentIsHtml = isHtml,
                    ReportedUrl = url
                });
            }
            return list;
        }

        // 取得未審核檢舉
        public List<ReportDto> GetReports(string? searchText, int skip, int take)
        {
            var baseQuery = _context.Reports.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                baseQuery = baseQuery.Where(r =>
                    r.Reason.Contains(searchText) ||
                    r.Description.Contains(searchText) ||
                    r.ReportedType.Contains(searchText));
            }

            var page = baseQuery.OrderByDescending(r => r.CreatedAt).Skip(skip).Take(take).ToList();

            var postIds = page.Where(r => r.ReportedType == "Post")
                .Select(r => int.TryParse(r.ReportedId, out var id) ? id : (int?)null)
                .Where(id => id.HasValue).Select(id => id!.Value).ToList();

            var commentIds = page.Where(r => r.ReportedType == "Comment")
                .Select(r => int.TryParse(r.ReportedId, out var id) ? id : (int?)null)
                .Where(id => id.HasValue).Select(id => id!.Value).ToList();

            var posts = _context.Posts.Where(p => postIds.Contains(p.Id)).ToDictionary(p => p.Id);
            var comments = _context.Comments.Where(c => commentIds.Contains(c.Id)).ToDictionary(c => c.Id);

            var list = new List<ReportDto>();
            foreach (var r in page)
            {
                string? title = null;
                string? content = null;
                bool isHtml = false;
                string? url = null;

                if (r.ReportedType == "Post" && int.TryParse(r.ReportedId, out var pid))
                {
                    if (posts.TryGetValue(pid, out var p) && p != null)
                    {
                        title = (p.GetType().GetProperty("Title")?.GetValue(p) as string) ?? "貼文";
                        (content, isHtml) = TryGetPostContent(p);
                        url = $"/Posts/Detail/{pid}";
                    }
                    else title = "[已刪除]";
                }
                else if (r.ReportedType == "Comment" && int.TryParse(r.ReportedId, out var cid))
                {
                    if (comments.TryGetValue(cid, out var c) && c != null)
                    {
                        title = "留言";
                        content = c.Content;
                        isHtml = false;
                        url = $"/Comments/Detail/{cid}";
                    }
                    else title = "[已刪除]";
                }

                list.Add(new ReportDto
                {
                    Id = r.Id,
                    ReporterId = r.ReporterId,
                    ReportedType = r.ReportedType,
                    ReportedId = int.TryParse(r.ReportedId, out var rid) ? rid : 0,
                    Reason = r.Reason,
                    Description = r.Description,
                    Status = r.Status,
                    ResolutionNotes = r.ResolutionNotes,
                    ResolvedAt = r.ResolvedAt,
                    CreatedAt = r.CreatedAt,

                    ReportedTitle = title ?? "[已刪除]",
                    ReportedContent = string.IsNullOrWhiteSpace(content) ? null : content,
                    ReportedContentIsHtml = isHtml,
                    ReportedUrl = url
                });
            }
            return list;
        }

 // 更新檢舉狀態
        public bool UpdateReportStatus(int reportId, string status, string? resolutionNotes = null)
        {
            var report = _context.Reports.FirstOrDefault(r => r.Id == reportId);
            if (report == null)
                return false;

            report.Status = status;
            report.ResolutionNotes = resolutionNotes;

            // 如果審核完成 存台灣時間
            if (status == "Resolved" || status == "Rejected")
                report.ResolvedAt = DateTime.UtcNow.AddHours(8);

            _context.SaveChanges();
            return true;
        }
  
        /// <summary>
        /// 嘗試從貼文實體抓出內文，容錯常見欄位名稱；回傳（內容, 是否為Html）
        /// </summary>
        private static (string? content, bool isHtml) TryGetPostContent(object postEntity)
        {
            // 依序嘗試的欄位名稱
            var htmlCandidates = new[] { "ContentHtml", "HtmlContent" };
            var textCandidates = new[] { "Content", "Body", "Description", "Text" };

            var type = postEntity.GetType();

            // 先試 HTML 欄位
            foreach (var name in htmlCandidates)
            {
                var prop = type.GetProperty(name);
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    var val = prop.GetValue(postEntity) as string;
                    if (!string.IsNullOrWhiteSpace(val)) return (val, true);
                }
            }
            // 再試純文字欄位
            foreach (var name in textCandidates)
            {
                var prop = type.GetProperty(name);
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    var val = prop.GetValue(postEntity) as string;
                    if (!string.IsNullOrWhiteSpace(val)) return (val, false);
                }
            }
            return (null, false);
        }


    }
}
