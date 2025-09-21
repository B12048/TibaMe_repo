using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    public class ManufacturerController : Controller
    {
        public IActionResult Facturer()
        {
            return View();
        }
    }
}
