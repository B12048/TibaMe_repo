using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Entity
{
    public class TempUser //這邊是使用者點選註冊後，進入驗證信驗證狀態的資料暫時存放區
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        [Range(0,2)]
        public int Gender { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Token { get; set; } = Guid.NewGuid().ToString(); // 驗證用
        public DateTime ExpireAt { get; set; } = DateTime.Now.AddMinutes(5); // 有效期限
    }
}
