using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    public class MyPageController : Controller
    {
        public IActionResult MyProducts() => View();
        public IActionResult NewProduct() => View();
        public IActionResult EditProduct() => View();



    }
}
