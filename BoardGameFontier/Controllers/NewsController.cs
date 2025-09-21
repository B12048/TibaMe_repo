using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BoardGameFontier.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public NewsController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<News> objDiaryEntryList = _db.News.OrderByDescending(t => t.Created).Take(10).ToList();
            return View(objDiaryEntryList);
        }
        

        [HttpGet]
        public IActionResult Detail(int id)
        {
            //根據id找到新聞詳細
            News news = _db.News.FirstOrDefault(g => g.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            List<NewsComment> comment = _db.NewsComment.Where(c => c.NewsId == id).Include(c => c.Commenter).ToList();
            NewsViewModel vm = new NewsViewModel
            {
                news = news,
                NewsComment = comment
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<List<NewsCommentDTO>> GetNewsComments(int id)
        {
            return await _db.NewsComment.Where(c => c.NewsId == id)
                .Select(c => new NewsCommentDTO
                {
                    Content = c.Content,
                    Created = c.Created,
                    DisplayName = c.Commenter.DisplayName,
                    ProfilePictureUrl = c.Commenter.ProfilePictureUrl
                })
                .ToListAsync();
        }

        [HttpPost]
        public IActionResult SubmitComment([FromBody] NewsCommentDTO dto)
        {
            var user  = _db.UserProfiles.FirstOrDefault(u=>u.UserName== dto.UserName);
            var comment = new NewsComment
            {
                NewsId = dto.NewsId,
                CommenterId = user.Id,
                Content = dto.Content,
                Created = DateTime.Now
            };
            _db.NewsComment.Add(comment);
            _db.SaveChanges();


            //在這裡組一個假dto丟到前端，才可以順利放入資料陣列
            var savedComment = new NewsCommentDTO
            {
                NewsId = dto.NewsId,
                Created = DateTime.Now,
                UserName = dto.UserName,
                Content = dto.Content,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DisplayName = user.DisplayName
            };

            return Ok(savedComment);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateTest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(News obj)
        {
            if (ModelState.IsValid)
            {
                _db.News.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Publish", "Admin"); //多載("action","controller")
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            News? News = _db.News.Find(Id);
            if (News == null)
            {
                return NotFound();
            }
            return View(News);
        }

        [HttpPost]
        public IActionResult Edit(News obj)
        {
            if (ModelState.IsValid)
            {
                _db.News.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Publish", "Admin"); //多載("action","controller")
            }
            return View(obj);
        }


        [HttpGet]
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            News? News = _db.News.Find(Id);
            if (News == null)
            {
                return NotFound();
            }
            return View(News);
        }

        [HttpPost]
        public IActionResult Delete(News obj)
        {
            _db.News.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Publish", "Admin"); //多載("action","controller")
        }
    }
}
