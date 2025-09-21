using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BoardGameFontier.Services
{
    public class CurrentUserService : ICurrentUserService
    {               //HttpContextAccessor是目前這個請求的使用者狀態
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        //這邊我來做個整理
        //_httpContextAccessor:用來存取「目前這個網頁請求的 HttpContext」，也就是「這個連線的用戶」。
        // HttpContext.User：這是目前登入使用者的「身分（ClaimsPrincipal）」


        // 透過 ClaimsPrincipal (HttpContext.User) 來取得使用者ID
        // ClaimTypes.NameIdentifier登入憑證內的標準欄位，通常代表「使用者ID」
        public string? UserId => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 透過 Identity 來取得使用者名稱
        public string? UserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;

        // 判斷 Identity 是否已驗證
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public async Task<UserProfile?> GetUserProfileAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return null; // 如果沒有 UserId，直接返回 null
            }

            // 使用 UserId 從資料庫中查詢完整的 UserProfile 物件
            // 使用快取或更高效的方式來避免每次都查詢資料庫，是進一步的優化方向
            return await _dbContext.UserProfiles
                                   .FirstOrDefaultAsync(u => u.Id == UserId);
        }

        public async Task<List<string>> GetRolesAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return new List<string>(); //這段主要用來防呆

            return await _dbContext.CustomUserRoles
                .Where(u => u.UserId == UserId)
                .Select(u => u.RoleName)
                .ToListAsync(); 
        }

        public async Task<bool>IsInRoleAsync(string roleName)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(roleName))
                return false; //這段主要用來防呆
            return await _dbContext.CustomUserRoles
                .AnyAsync(u => u.UserId == UserId && u.RoleName == roleName);
        }

        public async Task<string> GetUserDisplayNameAsync()
        {
            var userId = UserId;
            var username = UserName;           

            var displayName = await _dbContext.UserProfiles
                .AsNoTracking() //純讀取不追蹤!
                .Where(u => u.Id == userId)
                .Select(u => u.DisplayName)
                .SingleOrDefaultAsync();

            return displayName ?? "";  // username
        }
    }
}
