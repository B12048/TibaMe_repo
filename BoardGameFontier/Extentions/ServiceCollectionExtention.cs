using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Repository;
using BoardGameFontier.Services;
using Microsoft.EntityFrameworkCore;

namespace BoardGameFontier.Extentions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 服務的集合
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            AddTransientServices(services);
            AddScopedServices(services);
            AddSingletonServices(services);
            return services;
        }

        private static void AddTransientServices(IServiceCollection services)
        {
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IAnalyticsService, AnalyticsService>();
            services.AddTransient<IProductService, ProductService>();
        }

        private static void AddScopedServices(IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // 社群功能服務
            services.AddScoped<ICommunityService, CommunityService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ILikeService, LikeService>(); // 重構後的統一按讚服務
            services.AddScoped<IFollowService, FollowService>(); // 追蹤功能服務

            // 會員相關服務
            services.AddScoped<IMemberProfileService, MemberProfileService>();

            // 內容驗證和安全服務
            services.AddScoped<IContentValidationService, ContentValidationService>();

            // ↓ 密碼驗證(含加鹽雜湊)
            services.AddScoped<IPasswordService, PasswordService>();

            // ↓ 註冊或忘記密碼寄送驗證信
            services.AddScoped<GoogleMailService>();

            // Repository
            services.AddScoped<GameDetailsRepository>();
            services.AddScoped<NewsRepository>();
            services.AddScoped<ReelsRepository>();

            // ====== 新增：檢舉審核主服務（最小變更，僅此一行） ======
            services.AddScoped<IModerationService, ModerationService>();
        }

        private static void AddSingletonServices(IServiceCollection services)
        {
            // 快取服務註冊為 Singleton，確保應用程式生命週期內快取狀態一致
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<IUploadImageService, UploadImageService>();
        }
    }
}
