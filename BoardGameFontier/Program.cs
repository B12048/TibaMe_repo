using BoardGameFontier.Extentions;
using BoardGameFontier.Filters;
using BoardGameFontier.Hubs;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Repository;
using BoardGameFontier.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BoardGameFontier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== 資料庫連線設定 =====
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .ConfigureWarnings(warnings =>
                           warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning));
            });

            builder.Services.AddScoped<ApplicationDbContext>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                return factory.CreateDbContext();
            });

            // Session 設定
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".BoardGameFontier.Session";
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;

                if (builder.Environment.IsDevelopment())
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                }
                else
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }
            });

            // Hangfire
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage()
            );
            builder.Services.AddHangfireServer();

            // 其他服務
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder =>
                    builder.Expire(TimeSpan.FromSeconds(60))
                           .SetVaryByHeader("Cookie"));
                options.AddPolicy("Allday", builder =>
                    builder.Expire(TimeSpan.FromHours(24))
                           .SetVaryByHeader("Cookie"));
            });

            builder.Services.AddService();
            //↓給MVC(Razor)用前端驗證用
            builder.Services.AddControllersWithViews();
            // 註冊自訂的記憶體快取服務 ↓專門給API回傳JSON的內容
            builder.Services.AddControllers(options =>
            {
                // 全域註冊 ValidationActionFilter，統一處理 API 驗證錯誤
                options.Filters.Add<ValidationActionFilter>();
            });
            builder.Services.AddSignalR();
            builder.Services.AddMemoryCache();

            // ===== Cookie Authentication + 帳號鎖定檢查 =====
            builder.Services.AddScoped<MerchandiseRepository>();
            //Cookie - 優化設定確保登入登出狀態立即同步
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
          .AddCookie(option =>
          {
              option.AccessDeniedPath = "/forbidden";
              option.LoginPath = "/Login/IndexLogin";
              option.LogoutPath = "/Login/Logout";
              option.ExpireTimeSpan = TimeSpan.FromDays(30);
              option.SlidingExpiration = true;

              if (builder.Environment.IsDevelopment())
              {
                  option.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
              }
              else
              {
                  option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
              }

              option.Cookie.SameSite = SameSiteMode.Lax;
              option.Cookie.HttpOnly = true;

              // 🔑 這裡就是檢查帳號是否被鎖的地方
              option.Events = new CookieAuthenticationEvents
              {
                  OnValidatePrincipal = async context =>
                  {
                      var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                      var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

                      if (!string.IsNullOrEmpty(userId))
                      {
                          var user = await db.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);

                          if (user == null || !user.LockoutEnabled)
                          {
                              // 帳號被鎖 → 立即登出 + 導回登入頁
                              context.RejectPrincipal();
                              await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                              context.Response.Redirect("/Login/IndexLogin?locked=true");
                          }
                      }
                  }
              };
          });
            ;

            var app = builder.Build();

            // Middleware pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            //  app.UseForwardedHeaders...這一段是讓網站在「代理/雲端環境」(例如 Azure) 下
            // 仍然能正確知道：
            // 1. 使用者真正的 IP (X-Forwarded-For)           
            // 2. 使用者真正用的協定 http / https (X-Forwarded-Proto)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard();
            app.UseOutputCache();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<UserHub>("/hubs/userCount");
            app.MapHub<ChatHub>("/hubs/chat");
            app.MapHub<PrivateChatHub>("/hubs/privateChat");

            app.Run();
        }
    }
}
