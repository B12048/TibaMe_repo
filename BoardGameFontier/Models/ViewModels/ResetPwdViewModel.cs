using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Models.ViewModels
{
    public class ResetPwdViewModel
    {
       [ Required(ErrorMessage = "尚未填寫新密碼")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "未填寫確認密碼")]
        public string? ConfirmPassword { get; set; }

        [Required] //多這個可以直接在Controller/Hidden 欄位都能直接用
        public string? Token { get; set; } //這邊是用來接收重設密碼的Token
    }
}
