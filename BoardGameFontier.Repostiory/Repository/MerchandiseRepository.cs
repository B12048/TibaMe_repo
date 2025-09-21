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
    public class MerchandiseRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _db;
        public MerchandiseRepository(IDbContextFactory<ApplicationDbContext> db)
        {
            _db = db;
        }

        public async Task<List<MerchandiseDTO>> GetMerchandiseAsync()
        {
            using var context = _db.CreateDbContext();

            return await context.Merchandise.AsNoTracking()
                .Include(m => m.GameDetail)
                .Select(
                    m => new MerchandiseDTO
                    {
                        Intro = m.GameDetail.Intro,
                        ZhtName = m.GameDetail.zhtTitle,
                        EngName = m.GameDetail.engTitle,
                        CoverURL = m.CoverURL,
                        Price = m.Price,
                        Stock = m.Stock,
                        IndexBannerURL = m.IndexBannerURL
                    }).ToListAsync();
        }
    }
}
