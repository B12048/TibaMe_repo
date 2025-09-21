using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory.Entity;

namespace BoardGameFontier.Services
{
    public interface IPasswordService
    {
        string GenerateSalt(int size = 16);
        string HashPassword(string password, string salt);
        bool VerifyPassword(string inputPassword, string? storeHash, string? storedSalt);

        TempUser CreateTempUser(RegisterViewModel model, string passwordHash, string salt, string token);
        UserProfile CreateUerProfile(TempUser tempUser);
        UserRole CreateUserRole(string userId);

        void SetResetPasswordToken(UserProfile user);
        string ResetPwdMailBody(string resetUrl);
        void UpdateUserPassword(UserProfile user, string newPassword);
    }
}
