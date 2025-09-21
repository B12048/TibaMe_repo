namespace BoardGameFontier.Constants
{
    public class UserRolesLevel
    { //const為常數，讓角色這個值本身可以好好固定住不會被更動
        //另外寫在這邊也是因為好統一管理，未來新增角色很方便
        public const string User = "User";
        public const string Gm = "Gm";
        public const string Admin = "Admin";
        public const string Manufacturer = "Manufacturer"; //廠商角色
    }
}
