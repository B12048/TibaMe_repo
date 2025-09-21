using BoardGameFontier.Repostiory.DTOS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Repository
{
    public class ReelsRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public ReelsRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public async Task<List<HomeReelsDto>> GetCurrentReelsAsync()
        {
            using var context = contextFactory.CreateDbContext();
            return await context.Reel.AsNoTracking()
                         .Where(t => t.CreatedAt >= DateTime.Now.AddDays(-365)).Select(x => new HomeReelsDto
                         {
                             UserName = x.User.DisplayName,
                             ImageURL = x.ImageURL,
                         }).ToListAsync();
        }
    }
}
