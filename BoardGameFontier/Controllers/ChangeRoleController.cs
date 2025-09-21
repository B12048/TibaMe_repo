using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameFontier.Controllers
{                                               //此地方為AI產生，我暫時拿來新增角色用
    public class ChangeRoleController : Controller 
    {
        private readonly ApplicationDbContext _context;

        public ChangeRoleController(ApplicationDbContext context)
        {
            _context = context;
        }
                                            //這邊的? 是用來分開前後的 前面是找路徑，後面是查詢語法
        // 範例網址：http://localhost:5000/ChangeRole/AddRole?userName=aaa@gmail.com&roleName=Gm
        public async Task<IActionResult> AddRole(string userName, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("userName 與 roleName 不能為空");
            }

            var user = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return NotFound($"找不到帳號：{userName}");
            }

            // 檢查該角色是否已經存在
            var hasRole = await _context.CustomUserRoles
                .AnyAsync(r => r.UserId == user.Id && r.RoleName == roleName);

            if (hasRole)
            {
                return Content($"使用者 {userName} 已經擁有角色 {roleName}");
            }

            _context.CustomUserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleName = roleName,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Content($"✅ 使用者 {userName} 成功新增角色：{roleName}");
        }
    }
}
