using BoardGameFontier.Repostiory.DTOS;

namespace BoardGameFontier.Services
{
    public interface IReportService
    {
        // 新增檢舉，直接存台灣時間
        bool CreateReport(string reportedType, int reportedId, string reason, string? description);

        // 取得報表資料
        List<ReportDto> GetReports(string? searchText, int skip, int take);

        // 更新檢舉狀態，審核完成時存台灣時間到 ResolvedAt
        bool UpdateReportStatus(int reportId, string status, string? resolutionNotes = null);

        // 取得未審核的檢舉
        List<ReportDto> GetUnreviewedReports();
    }
}
