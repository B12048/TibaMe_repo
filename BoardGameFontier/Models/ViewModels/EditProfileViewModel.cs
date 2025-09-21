using BoardGameFontier.Repostiory.Entity;
using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Models.ViewModels
{
    //public EditProfileViewModel(UserProfile? userProfile) { }
    public class EditProfileViewModel
    {
        public string? ProfilePictureUrl { get; set; }

        [StringLength(12)]
        public string DisplayName { get; set; } = "";

        public string? City { get; set; }

        public List<string>? BoardGameTags { get; set; } = new();  // 前端以字串陣列傳遞

        [StringLength(300)]
        public string? Bio { get; set; }

        // 密碼編輯（非必填，填寫才處理）

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "兩次密碼不一致")]
        public string? ConfirmPassword { get; set; }
    }
}
