using BoardGameFontier.Constants;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory.Entity;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace BoardGameFontier.Services
{
    public class PasswordService : IPasswordService
    {
        //這邊產亂數Salt
        public string GenerateSalt(int size = 16)
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[size];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        //這兒做雜湊
        public string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        //驗證密碼（登入時用）
        public bool VerifyPassword(string inputPassword, string? storeHash, string? storedSalt)
        {                                                     //↑這邊是資料庫存的雜湊
            if (string.IsNullOrEmpty(storeHash) || string.IsNullOrEmpty(storedSalt))
                return false;
            var hashOfInput = HashPassword(inputPassword, storedSalt);
            return hashOfInput == storeHash;
        }

        //驗證時暫時寫入的資料表
        //TempUser回傳的是物件
        public TempUser CreateTempUser(RegisterViewModel model, string passwordHash, string salt, string token)

        {
            return new TempUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                Birthday = model.BirthDate,
                Gender = model.Gender,
                CreatedAt = DateTime.Now,
                Token = token,
                ExpireAt = DateTime.Now.AddMinutes(5)
            };
        }

        //使用者驗證過後將使用者資料從TempUser資料表轉到正式的UserProfile資料表
        public UserProfile CreateUerProfile(TempUser tempUser)
        {
            return new UserProfile
            {
                Id = Guid.NewGuid().ToString(),
                UserName = tempUser.UserName,
                PasswordHash = tempUser.PasswordHash,
                PasswordSalt = tempUser.PasswordSalt,
                Birthday = tempUser.Birthday,
                Gender = tempUser.Gender,
                CreatedAt = tempUser.CreatedAt,
                UpdatedAt = DateTime.Now,
                EmailConfirmed = true, //這裡是在標記此帳號有信箱驗證成功
                IsDeleted = false,
                DeletedAt = null
            };
        }

        public UserRole CreateUserRole(string userId)
        {
            return new UserRole
            {
                UserId = userId,
                RoleName = UserRolesLevel.User, //所有一般使用者一開始就都是User角色
                CreatedAt = DateTime.Now
            };
        }

        //設定重設密碼的Token
        public void SetResetPasswordToken(UserProfile user)
        {     //↑直接修改物件內容所以不用回傳
            user.ResetPwdToken = Guid.NewGuid().ToString(); //產生一個新的Token
            user.ResetPwdTokenExpires = DateTime.Now.AddMinutes(5); //有效期限5分鐘
        }


        //寄信內容
        public string ResetPwdMailBody(string resetUrl)
        {
            return $@"
            <p>您好：</p>
            <p>請點擊下方連結以重設您的密碼：</p>
            <p><a href='{resetUrl}'>點我重設密碼</a></p>
            <p>本連結將於5分鐘後失效。</p>
            <p>如果大大沒有提出重設密碼的請求，請忽略此信。</p> ";
        }

        //直接修改物件內容所以不用回傳
        public void UpdateUserPassword(UserProfile user, string newPassword)
        {
            string newSalt = GenerateSalt(); //產生新的Salt
            string newHash = HashPassword(newPassword, newSalt); //雜湊新密碼

            user.PasswordSalt = newSalt; //更新Salt
            user.PasswordHash = newHash; //更新雜湊密碼

            //清除大隊
            user.ResetPwdToken = null; 
            user.ResetPwdTokenExpires = null; 

        }


    }

}
