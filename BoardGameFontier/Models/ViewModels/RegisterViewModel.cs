using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "電子郵件格式錯誤")]
        public string UserName { get; set; } = string.Empty;
       
        [Required(ErrorMessage = "請輸入密碼")]
        //[MinLength(6, ErrorMessage = "密碼最少輸入6碼")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "請輸入確認密碼")]
        [Compare("Password",ErrorMessage = "密碼與確認密碼不一致")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required (ErrorMessage = "請選擇性別")]
        [Range(1, 2)]
        public int Gender { get; set; } 


        [Required(ErrorMessage = "請選擇出生日期")]
        public DateTime BirthDate { get; set; } 
    }
}
