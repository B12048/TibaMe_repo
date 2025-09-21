using BoardGameFontier.Repostiory;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index() => RedirectToAction("Analyze");
        public ActionResult Analyze() => View();
        public ActionResult Contact() => View();
        public ActionResult Publish()
        {
            var news = _db.News.OrderByDescending(t => t.Created).ToList();
            return View(news);
        }
        public ActionResult Manage() => View();
    }
}
