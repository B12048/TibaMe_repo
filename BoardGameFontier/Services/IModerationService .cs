using System.Threading.Tasks;

namespace BoardGameFontier.Services
{
    public interface IModerationService
    {
        /// <summary>審核通過：發通知、標記已處理、計數、滿五次鎖帳</summary>
        Task ApproveReportAsync(int reportId, string? notes);

        /// <summary>審核退回：發通知、標記退回</summary>
        Task RejectReportAsync(int reportId, string? notes);
    }
}
