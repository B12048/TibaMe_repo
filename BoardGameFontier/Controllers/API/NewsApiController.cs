using BoardGameFontier.DTOs;
using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BoardGameFontier.Controllers.API
{
    [Route("api/news")]
    [ApiController]
    [OutputCache]
    public class NewsApiController : ControllerBase
    {
        private readonly NewsRepository _newsRepository;

        public NewsApiController(NewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        [HttpGet("getnews")]
        public async Task<List<HomeNewsDto>> GetNews(string category)
        {
            return await _newsRepository.GetCurrentNewsAsync(category);
        }
    }
}
