using BoardGameFontier.Repostiory.Entity;
using System.Linq;
namespace BoardGameFontier.Extensions
{
    public static class QueryableExtensions
    {
        // 統一過濾「未刪」的使用者
        public static IQueryable<UserProfile> WhereActive(this IQueryable<UserProfile> query)
            => query.Where(u => !u.IsDeleted);
    }
}
