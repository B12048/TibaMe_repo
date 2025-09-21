using BoardGameFontier.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Repository
{
    public class GameDetailsRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public GameDetailsRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public async Task<List<HottestGameDto>> GetHotGamesAsync(int take = 4)
        {
            //todo Where(record => record.ClickedTime >= DateTime.Now.AddDays(-7)) //這個條件好像太難了，晚點再實施
            using var context = contextFactory.CreateDbContext();
            return await context.GameClickLog.AsNoTracking()
                .GroupBy(t => t.GameDetailId)
                .Select(g => new
                {
                    GameDetailId = g.Key,
                    ClickCount = g.Count()
                })
                .OrderByDescending(group => group.ClickCount)
                .Take(take)
                .Join(context.GameDetails.AsNoTracking(),
                click => click.GameDetailId,
                game => game.Id,
                (click, game) => new HottestGameDto
                {
                    Id = game.Id,
                    Cover = game.Cover,
                    engTitle = game.engTitle,
                    zhtTitle = game.zhtTitle,
                })
                .ToListAsync();
        }
    }
}
