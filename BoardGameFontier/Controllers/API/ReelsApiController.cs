using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BoardGameFontier.Controllers.API
{
    [Route("api/Reels/[action]")]
    [ApiController]
    [OutputCache]
    public class ReelsApiController : ControllerBase
    {
        private readonly ReelsRepository _reelsRepository;

        public ReelsApiController(ReelsRepository reelsRepository)
        {
            _reelsRepository = reelsRepository;
        }


        public async Task<List<HomeReelsDto>> GetReels()
        {
            return await _reelsRepository.GetCurrentReelsAsync();
        }
    }
}
