using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory.Entity;
using Newtonsoft.Json;

namespace BoardGameFontier.Services
{
    public class UpdateProfileResult  //這邊先建立欄位實作好處理
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public UserProfile UpdateUser { get; set; }
    }
    public class UpdatePrivacyResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = "";
    }

    public interface IMemberProfileService
    {
        MemberIndexViewModel GetMemberProfile(UserProfile userProfile);
        EditProfileViewModel GetEditProfile(UserProfile userProfile);
        PrivacySettingsViewModel GetPrivacySettings(UserProfile userProfile);

        //這邊有刻意把資料庫跟前端呈現的做分離                                             //這邊還是要問一下為甚麼要用介面的方式注入
        UpdateProfileResult UpdateProfile(UserProfile user, EditProfileViewModel model, IPasswordService passwordService);

        UpdatePrivacyResult UpdatePrivacy(UserProfile user, string field, bool value);
        Task<PlayerProfileViewModel?> GetPlayerProfileAsync(string? id, string? userName);
        Task<(bool ok, string? err)> SoftDeleteAccountAsync(string userName);
       
    }
}
