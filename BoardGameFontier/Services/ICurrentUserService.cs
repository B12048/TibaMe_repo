using BoardGameFontier.Repostiory.Entity;

namespace BoardGameFontier.Services
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// 取得當前登入使用者的唯一ID (對應到 UserProfile.Id)。
        /// 如果未登入，則返回 null。
        /// </summary>
        string? UserId { get; }

        /// <summary>
        /// 取得當前登入使用者的名稱 (對應到 User.Identity.Name)。
        /// 如果未登入，則返回 null。
        /// </summary>
        string? UserName { get; }

        /// <summary>
        /// 判斷使用者是否已經通過驗證 (登入)。
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 從資料庫非同步取得完整的使用者個人資料物件。
        /// 如果未登入，則返回 null。
        /// </summary>
        Task<UserProfile?> GetUserProfileAsync();




        Task<List<string>> GetRolesAsync();

        Task<bool> IsInRoleAsync(string roleName);

        Task<string> GetUserDisplayNameAsync();
    }
}
