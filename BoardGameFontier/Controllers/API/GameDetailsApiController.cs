using BoardGameFontier.DTOs;
using BoardGameFontier.Repostiory.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BoardGameFontier.Controllers.API
{
    [Route("api/GameDetails/[action]")]
    [ApiController]
    [OutputCache(PolicyName = "Allday")]
    public class GameDetailsApiController : ControllerBase
    {
        private readonly GameDetailsRepository _gameDetailsRepository;

        public GameDetailsApiController(GameDetailsRepository gameDetailsRepository)
        {
            _gameDetailsRepository = gameDetailsRepository;
        }


        [HttpGet("{take}")]
        public async Task<List<HottestGameDto>> GetGameDetails(int take)
        {
            return await _gameDetailsRepository.GetHotGamesAsync(take);
        }
    }
}
