using BoardGameFontier.Models;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BGProjectTest.Controllers
{
    public class DexController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DexController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Manage()
        {
            List<GameDetail> objGameDetailList = _db.GameDetails.ToList();
            return View(objGameDetailList);
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<GameDetail> objGameDetailList = _db.GameDetails.ToList();
            return View(objGameDetailList);
        }


        [HttpGet]
        public IActionResult Detail(int id)
        {
            var game = _db.GameDetails.FirstOrDefault(g => g.Id == id);
            if(game == null)
            {
                return NotFound();
            }

            var vm = new DexViewModel
            {
                GameDetails = game,
                Ratings = _db.Ratings.Where(r => r.GameId == id).ToList()
            };

            //每次點擊時，建立一筆Click物件
            var clickLog = new GameClickLog
            {
                GameDetailId = game.Id,
                ClickedTime = DateTime.Now
            };

            //然後將Click物件加入到資料庫後，更新資料庫
            _db.GameClickLog.Add(clickLog);
            _db.SaveChanges();         

            return View(vm);
        }


        [HttpGet]
        public IActionResult Rating(int id)
        {
            var game = _db.GameDetails.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            var vm = new DexViewModel
            {
                GameDetails = game,
                Ratings = _db.Ratings.Where(r => r.GameId == id).ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(GameDetail obj)
        {
            if (obj != null && obj.engTitle.Length < 3)
            {
                ModelState.AddModelError("engTitle", "標題太短了");
            }
            if (ModelState.IsValid)
            {
                _db.GameDetails.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index","Dex"); //多載("action","controller")
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if(Id == null || Id == 0)
            {
                return NotFound();
            }
            GameDetail? gameDetail = _db.GameDetails.Find(Id);
            if(gameDetail == null)
            {
                return NotFound();
            }
            return View(gameDetail);
        }

        [HttpPost]
        public IActionResult Edit(GameDetail obj)
        {
            if (obj != null && obj.engTitle.Length < 3)
            {
                ModelState.AddModelError("engTitle", "標題太短了");
            }
            if (ModelState.IsValid)
            {
                _db.GameDetails.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Dex"); //多載("action","controller")
            }
            return View(obj);
        }
    }
}