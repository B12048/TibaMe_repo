using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    public class MarketController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Detail() => View();

    }
}