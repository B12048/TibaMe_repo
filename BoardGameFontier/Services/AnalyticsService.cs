using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.DTOS;

namespace BoardGameFontier.Services
{
    public class AnalyticsService: IAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public SummaryCardDto GetSummary()
        {
            var now = DateTime.UtcNow; // 或 DateTime.Now，看你 DB 時區
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            // 總文章數
            var totalNews = _context.News.Count();

            // 本月新增文章數（自動隨月份變動）
            var monthlyNews = _context.News
                .Count(n => n.Created >= startOfMonth && n.Created < startOfNextMonth);

            // 總瀏覽數
            var totalWatch = _context.News.Sum(n => n.PageView);

            return new SummaryCardDto
            {
                totalNews = totalNews.ToString(),
                monthlyNews = monthlyNews.ToString(),
                totalWatch = totalWatch.ToString()
            };
        }
        /// <summary>
        /// 圖形和折線圖
        /// </summary>
        /// <returns></returns>
        public ChartDto GetPostTrend()
        {
            var now = DateTime.UtcNow; // 或 DateTime.Now
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var query = _context.News
                .Where(n => n.Created >= startOfMonth && n.Created < startOfNextMonth)
                .GroupBy(n => n.Created.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return new ChartDto
            {
                Labels = query.Select(q => q.Date.ToString("MM-dd")).ToList(),
                Data = query.Select(q => q.Count).ToList()
            };
        }
        /// <summary>
        /// 圖形和折線圖
        /// </summary>
        /// <returns></returns>
        public ChartDto GetCategoryDistribution()
        {
            var result = _context.News
            .GroupBy(n => n.category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count()
            })
            .ToList();

            return new ChartDto
            {
                Labels = result.Select(r => r.Category).ToList(),
                Data = result.Select(r => r.Count).ToList()
            };
        }
    }
}
