using BoardGameFontier.Repostiory.DTOS;

namespace BoardGameFontier.Services
{
    public interface IAdminService
    {
        List<ContactAppDto> GetAllUsers(string searchText, int skip, int take);

        bool UpdateUserLockout(string id, bool lockoutEnabled);

        bool UpdateUserRole(string id, string role);
    }

}
