using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    public class CreditsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
