using BoardGameFontier.Extensions;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 管理後台服務
    /// 約定：UpdateUserLockout(id, lockoutEnabled)
    ///   - lockoutEnabled == false => 鎖帳（禁止登入，LockoutEnd = 7 天後）
    ///   - lockoutEnabled == true  => 解鎖（允許登入，LockoutEnd = NULL）
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        // 你要的鎖定期間（天）
        private const int LockDays = 7;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 取得所有使用者
        public List<ContactAppDto> GetAllUsers(string searchText, int skip, int take)
        {
            var query =
                _context.UserProfiles
                .WhereActive()
                .Join(_context.CustomUserRoles,
                      user => user.Id,
                      role => role.UserId,
                      (user, role) => new ContactAppDto
                      {
                          Display = user.DisplayName,
                          LockoutEnabled = user.LockoutEnabled, // false=鎖、true=可登入
                          Roles = role.RoleName,
                          UserName = user.UserName,
                          Id = user.Id
                      });

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(x =>
                    x.UserName.Contains(searchText) ||
                    x.Display.Contains(searchText));
            }

            return query
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        /// <summary>
        /// 更新帳號鎖定狀態
        /// lockoutEnabled == false => 鎖帳（LockoutEnd = 現在 + 7 天）
        /// lockoutEnabled == true  => 解鎖（LockoutEnd = NULL）
        /// </summary>
        public bool UpdateUserLockout(string id, bool lockoutEnabled)
        {
            var user = _context.UserProfiles.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;

            // 1) 用 LockoutEnabled 當「是否允許登入」
            user.LockoutEnabled = lockoutEnabled;

            // 2) 若有 LockoutEnd 欄位，依據 lockoutEnabled 設定 7 天的鎖定或清空
            try
            {
                var prop = user.GetType().GetProperty("LockoutEnd");
                if (prop != null)
                {
                    DateTimeOffset? until =
                        lockoutEnabled ? (DateTimeOffset?)null : DateTimeOffset.UtcNow.AddDays(LockDays);

                    prop.SetValue(user, until);
                }
            }
            catch
            {
                // 若沒有 LockoutEnd 也無妨，僅靠 LockoutEnabled 控制登入。
            }

            _context.SaveChanges();
            return true;
        }

        // 更新使用者角色（維持你原本邏輯）
        public bool UpdateUserRole(string id, string role)
        {
            var userRole = _context.CustomUserRoles.FirstOrDefault(r => r.UserId == id);
            if (userRole == null) return false;

            userRole.RoleName = role;
            _context.SaveChanges();
            return true;
        }
    }
}
