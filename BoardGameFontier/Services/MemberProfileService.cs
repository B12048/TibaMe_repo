using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BoardGameFontier.Services
{
    public class MemberProfileService(ApplicationDbContext _dbContext,ICurrentUserService _current) : IMemberProfileService
    {
        public async Task<(bool ok, string? err)> SoftDeleteAccountAsync(string userName)
        { //Tuple寫法(bool ok, string? err)
          // 找出這個使用者（沒加 WhereActive，因為要能刪除已經存在的帳號）
          // 直接用 UserId 找目前登入者
            var user = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(u => u.Id == _current.UserId && !u.IsDeleted);

            if (user == null)
            {
                return (false, "找不到使用者或帳號已刪除");
            }

            // 標記為刪除 + 記錄刪除時間
            user.IsDeleted = true;
            user.DeletedAt = DateTime.Now;

            // 這邊不特別做匿名化
            //user.DisplayName = "[已刪除使用者]";
            //user.Bio = "";
            //user.City = "";
            //user.BoardGameTags = "[]";
            try
            {
                await _dbContext.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"刪除時發生錯誤：{ex.Message}");
            }
        }
        public PrivacySettingsViewModel GetPrivacySettings(UserProfile userProfile)
        {
            return new PrivacySettingsViewModel
            {
                IsCityHide = userProfile?.IsCityHide ?? false,
                IsProfileHide = userProfile?.IsProfileHide ?? false
            };
        }
        //拿來顯示給畫面(ViewModel)用的
        public MemberIndexViewModel GetMemberProfile(UserProfile userProfile)
        {
            return new MemberIndexViewModel
            {
                ProfilePictureUrl = userProfile.ProfilePictureUrl,
                DisplayName = userProfile.DisplayName,
                UserName = userProfile.UserName,
                City = userProfile.City,
                //標籤通常是存成JSON格式字串，所以需要把它重新轉成List
                BoardGameTags = !string.IsNullOrEmpty(userProfile.BoardGameTags)
                    ? JsonConvert.DeserializeObject<List<string>>(userProfile.BoardGameTags)
                    : new List<string>(),
                Bio = userProfile.Bio
            };
        }

        public EditProfileViewModel GetEditProfile(UserProfile userProfile)
        {
            return new EditProfileViewModel 
            {
                ProfilePictureUrl = userProfile.ProfilePictureUrl,
                DisplayName = userProfile.DisplayName,
                City = userProfile.City,
                //標籤通常是存成JSON格式字串，所以需要把它重新轉成List
                //Deserialize=反序列化  把存入檔案的字串==轉換==>物件/陣列
                BoardGameTags = !string.IsNullOrEmpty(userProfile.BoardGameTags)
                    ? JsonConvert.DeserializeObject<List<string>>(userProfile.BoardGameTags)
                    : new List<string>(),
                Bio = userProfile.Bio
            };
        }
        public PlayerProfileViewModel GetPlayerProfile(UserProfile userProfile)
        {
            return new PlayerProfileViewModel
            {
                IsProfileHide = userProfile.IsProfileHide,
                UserId = userProfile.Id,
                ProfilePictureUrl = userProfile.ProfilePictureUrl,
                DisplayName = userProfile.DisplayName,
                UserName = userProfile.UserName,
                City = userProfile.City,
                BoardGameTags = !string.IsNullOrEmpty(userProfile.BoardGameTags)
                    ? JsonConvert.DeserializeObject<List<string>>(userProfile.BoardGameTags)
                    : new List<string>(),
                Bio = userProfile.Bio
            };
        }

        public UpdateProfileResult UpdateProfile(
            UserProfile user,
            EditProfileViewModel model,
            IPasswordService passwordService)
        {
            var result = new UpdateProfileResult(); //建立結果物件            

            //這邊更新基本欄位
            user.DisplayName = model.DisplayName ?? ""; //避免null值導致錯誤
            user.Bio = model.Bio ?? "";
            user.City = model.City ?? "";


            //----------------↓此段解決JSON字串多包一層的問題-----------
            var tags = (model.BoardGameTags ?? []) //記一下[] 這是C#12才有的寫法
                .SelectMany(s => (s ?? "").Split(','))
                .Select(s => s.Trim()) //去空白的部分
                .Where(t => t.Length > 0)
                .Distinct()                
                .ToList();
            //----------------↑此段解決JSON字串多包一層-----------           

            //                                //Serialize=序列化  把物件/陣列 ==轉換==>存入檔案!    
            user.BoardGameTags = JsonConvert.SerializeObject(tags);
            user.UpdatedAt = DateTime.Now; // 更新最後修改時間

            //密碼變更部分 這邊邏輯似乎要更改一下不夠完整
            if (!string.IsNullOrWhiteSpace(model.OldPassword) &&
               !string.IsNullOrWhiteSpace(model.NewPassword) &&
               !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                // 驗證新舊密碼是否一致(確認密碼與新密碼用前端驗證)
                if (!passwordService.VerifyPassword(model.OldPassword, user.PasswordHash, user.PasswordSalt))

                {
                    result.Success = false;
                    result.ErrorMessage = "舊密碼不正確";
                    result.UpdateUser = null;
                    return result;
                }
                // 2. 驗證新密碼與確認新密碼一致(主要還是要用前端跳錯誤資訊)
                if (model.NewPassword != model.ConfirmPassword)
                {
                    result.Success = false;
                    result.ErrorMessage = "新舊密碼不一致母湯";
                    result.UpdateUser = null;
                    return result;
                }
                passwordService.UpdateUserPassword(user, model.NewPassword);
            }
            result.Success = true;
            result.ErrorMessage = "";
            result.UpdateUser = user; //將更新後的使用者資料回傳
            return result;

        }//在社群點使用者時要回傳的
        public async Task<PlayerProfileViewModel?> GetPlayerProfileAsync(string? id, string? userName)
        {
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(userName))
                return null;

            UserProfile? target = null;

            if (!string.IsNullOrWhiteSpace(id)) //讀Id
            {
                target = await _dbContext.UserProfiles
                    .AsNoTracking() //再次註記.AsNoTracking()純讀取、不打算更新
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            else if (!string.IsNullOrWhiteSpace(userName)) //或讀UserName
            {
                target = await _dbContext.UserProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == userName);
            }

            if (target == null) return null;

            // 直接重用原本的轉 ViewModel 方法
            return GetPlayerProfile(target);
        }

        public UpdatePrivacyResult UpdatePrivacy(UserProfile user, string field, bool value)
        {
            switch (field)
            {
                case "IsCityHide": user.IsCityHide = value; break;
                case "IsProfileHide": user.IsProfileHide = value; break;


                default:
                    return new UpdatePrivacyResult { Success = false, ErrorMessage = "更新失敗" };
            }
            
            user.UpdatedAt = DateTime.Now;
            return new UpdatePrivacyResult { Success = true };
        }
    }
}
