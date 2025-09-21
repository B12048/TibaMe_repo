using BoardGameFontier.Repostiory.DTOS;

namespace BoardGameFontier.Services
{
    public interface IAnalyticsService
    {
        SummaryCardDto GetSummary();
        ChartDto GetPostTrend();
        ChartDto GetCategoryDistribution();
    }
}
