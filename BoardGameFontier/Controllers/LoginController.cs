using BoardGameFontier.Constants;
using BoardGameFontier.Extensions;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BoardGameFontier.Controllers
{
    //這裡嘗試使用.NET8 C#12主建構子寫法
    public class LoginController(
    ApplicationDbContext _context,
    ICacheService _cacheService,
    IPasswordService _pwdService,
    GoogleMailService _mailService
        ) : Controller
    {
      
        public IActionResult IndexLogin(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexLogin(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            // 先找「未刪除」的帳號（能真正登入的那一筆）
            var user = await _context.UserProfiles
                .AsNoTracking() // AsNoTracking()只讀資料、不修改 → 不用追蹤，效能較好、記憶體省
                .FirstOrDefaultAsync(u => u.UserName == model.UserName && !u.IsDeleted);   

            // 若找不到未刪除帳號，再查一次：是否有「同信箱但已刪除」的紀錄
            if (user == null)
            {
                var wasDeleted = await _context.UserProfiles
                    .AsNoTracking() // 一樣只讀
                    .AnyAsync(u => u.UserName == model.UserName && u.IsDeleted);
                                                                    // ↑有這個信箱，但狀態是已刪除

                if (wasDeleted)
                {
                    // 有舊紀錄、但已刪除
                    ModelState.AddModelError(string.Empty, "此帳號已刪除，無法登入");
                    return View(model);
                }
                // 完全沒有這個帳號，統一回"帳號或密碼錯誤"
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            if (!_pwdService.VerifyPassword
              (model.Password, user.PasswordHash, user.PasswordSalt))//驗帳密
            {
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);//自己記一下string.Empty這不是針對某個欄位，而是整個表單的共用錯誤訊息
            }
            
            // 驗證帳號密碼成功後，加上 LockoutEnabled 檢查
            if (!user.LockoutEnabled)
            {
                ModelState.AddModelError(string.Empty, "違規達到上限,帳號已被鎖定");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }


            var Claims = new List<Claim> //身分證上的內容
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),//NameIdentifier名稱識別碼
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim("DisplayName", string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName : user.DisplayName)
            };

            var roles = await _context.CustomUserRoles
                .Where(u => u.UserId == user.Id)
                .Select(u => u.RoleName)
                .ToListAsync(); //注意因為下面有用到foreach迴圈撈資料所以ToList是一定要有

            foreach (var role in roles) //這邊用foreach迴圈是為了若之後要讓一個人有多角色可以用到
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }// Claims.Add( 建立一個身分證，且裡面的身分證的角色是甚麼)
             //ClaimTypes是ASP.NET來辨識「這是角色」

            //這邊在建立身分
            var claimsIdentity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                });

            // 清除使用者快取，確保登入狀態立即生效
            await ClearUserCacheAsync(user.Id);

            // 優化：登入成功後返回指定頁面，優先使用 returnUrl 參數
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            // 如果沒有 returnUrl，嘗試從 Referer 取得
            var refererUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refererUrl) && Url.IsLocalUrl(refererUrl))
            {
                return Redirect(refererUrl);
            }
            
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//記得前端有用到@Html.AntiForgeryToken()就要加
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isExist = await _context.UserProfiles.AnyAsync(u => u.UserName == model.UserName && !u.IsDeleted);
            if (isExist)
            {
                ModelState.AddModelError("UserName", "這個Email已經被註冊過");
                return View(model);
            }

            //↑基本驗證
            //----------------------------------------
            //這邊建立密碼跟Token
            var salt = _pwdService.GenerateSalt();
            var passwordHash = _pwdService.HashPassword(model.Password, salt);
            var token = Guid.NewGuid().ToString();

            //建立一個暫時的使用者資料表
            var tempUser = _pwdService.CreateTempUser(model, passwordHash, salt, token);
            _context.TempUsers.Add(tempUser);
            await _context.SaveChangesAsync();

            //這邊產生驗證的連結                             ↓ 這是一個匿名物件{參數名稱,變數}，但此處已簡化寫法              
            var verifyUrl = Url.Action("VerifyEmail", "Login", new { token }, Request.Scheme, Request.Host.Value); //Request.Scheme:目前網站的通訊協定
                                                                     // Request.Host.Value請用使用者現在正在瀏覽這個站的網域來組連結信件內容.寄信
                                                                     
            string body = _mailService.RegisterMailBody(verifyUrl);
            await _mailService.SendMailAsync(tempUser.UserName, "BoardGameFrontier の註冊驗證信", body);

            return View("RegisterConfirm"); 
        }

        [HttpGet] //使用者點擊超連結
        public async Task<IActionResult> VerifyEmail(string token) //這邊用在驗證mail過後寫入UserProfile
        {                                                     //這邊用Token是利用Token的唯一性來達到安全
            var tempUser = await _context.TempUsers.FirstOrDefaultAsync(u => u.Token == token);

            if (tempUser == null || tempUser.ExpireAt < DateTime.Now)
            //驗證信有效時間
            {
                return Content("驗證失敗：連結無效或已過期");//這邊在確認一下要不要用Content
            }           
            var newUser = _pwdService.CreateUerProfile(tempUser);  //這邊是將暫時的使用者資料轉成正式的使用者資料

            var newRole = _pwdService.CreateUserRole(newUser.Id); //這邊是建立一個新的使用者角色，預設給予一般使用者權限

            _context.CustomUserRoles.Add(newRole);
            _context.UserProfiles.Add(newUser);
            _context.TempUsers.Remove(tempUser); //移除暫存在tempUser資料表的內容
            await _context.SaveChangesAsync();

            return RedirectToAction("IndexLogin", "Login");
        }


        [HttpGet]
        public IActionResult ForgotPwd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //防CSRF記得前端有用到@Html.AntiForgeryToken()就要加
        public async Task<IActionResult>ForgotPwd(string email)
        {
            //找使用者帳號
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserName == email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "查無此Email");
                return View();
            }

            //設定重設密碼的Token
            _pwdService.SetResetPasswordToken(user);
            await _context.SaveChangesAsync();

            //產生重設密碼的連結
            var resetUrl = Url.Action("ResetPwd", "Login", new { token = user.ResetPwdToken }, Request.Scheme);

            //寄信給使用者
            var body = _pwdService.ResetPwdMailBody(resetUrl); 
            await _mailService.SendMailAsync(user.UserName, "BoardGameFrontier の密碼重設", body);

            return Content
                ("驗證信寄送成功！<br/>" +
                "<a href='/Login/IndexLogin'>回登入頁面</a>",
                "text/html; charset=utf-8");
        }

        [HttpGet]
        public IActionResult ResetPwd(string token)
        {
            var model = new ResetPwdViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //防CSRF記得前端有用到@Html.AntiForgeryToken()就要加
        public async Task<IActionResult> ResetPwd(ResetPwdViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _context.UserProfiles.FirstOrDefault(u => u.ResetPwdToken == model.Token);

            if (user == null ||
                user.ResetPwdTokenExpires == null || 
                user.ResetPwdTokenExpires < DateTime.Now)
            {
                ModelState.AddModelError("", "重設密碼連結無效或已過期");
                return View(model);
            }

            //更新密碼囉
            _pwdService.UpdateUserPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();

            return Content("密碼已重設成功！<br/>" +
                "<a href='/Login/IndexLogin'>回登入頁面</a>",
                "text/html; charset=utf-8");
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //記得前端有用到@Html.AntiForgeryToken()就要加
        public async Task<IActionResult> Logout() //登出
        {
            // 在登出前先取得當前使用者ID，用於清除快取
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //清除Cookie登出! 這邊因為Login就是用Cookie....Scheme這概念，所以登出也要
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 清除使用者快取，確保登出狀態立即生效
            if (!string.IsNullOrEmpty(currentUserId))
            {
                await ClearUserCacheAsync(currentUserId);
            }

            return RedirectToAction("Index", "Home"); 
            
        }

        /// <summary>
        /// 清除使用者相關快取
        /// </summary>
        private async Task ClearUserCacheAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            try
            {
                // 清除使用者基本資訊快取
                await _cacheService.RemoveAsync($"user_info_{userId}");
                
                // 清除使用者按讚相關快取
                await _cacheService.RemoveByPatternAsync($"user_likes_{userId}");
                await _cacheService.RemoveByPatternAsync($"user_stats_{userId}");
            }
            catch (Exception ex)
            {
                // 快取清除失敗不影響登入/登出流程
                Console.WriteLine($"清除快取失敗: {ex.Message}");
            }
        }

    }
}
