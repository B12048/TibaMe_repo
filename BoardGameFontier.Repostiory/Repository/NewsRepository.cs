using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Repository
{
    public class NewsRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public NewsRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public async Task<List<HomeNewsDto>> GetCurrentNewsAsync(string category)
        {
            using var context = contextFactory.CreateDbContext();
            var query = context.News.AsNoTracking().OrderByDescending(t => t.Created).Where(t=>t.status != "草稿");
            if (category != "all") 
            { 
                category = 翻譯蒟蒻(category);
                query = (IOrderedQueryable<News>)query.Where(t => t.category == category && t.status != "草稿");
            }
            return await SelectNews(query).Take(6).ToListAsync();
        }

        private static string 翻譯蒟蒻(string category)
        {
            switch (category)
            {
                case "news":
                    category = "遊戲趣聞";
                    break;
                case "game":
                    category = "新品速報";
                    break;
                case "campaign":
                    category = "實體活動";
                    break;
                case "announcement":
                    category = "系統公告";
                    break;
            }
            ;
            return category;
        }

        private IQueryable<HomeNewsDto> SelectNews(IQueryable<News> query) =>
        query.Select(x => new HomeNewsDto
        {
            Id = x.Id,
            Content = x.Content,
            CoverURL = x.CoverURL,
            Title = x.Title,
            Category = x.category
        });
    }
}
