using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameFontier.Controllers
{
    public class MallController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MallController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Manage()
        {
            var vm = await _context.Merchandise.Select(a => new MallManageViewModel
            {
                Id = a.Id,
                Name = a.GameDetail.zhtTitle,
                Price = a.Price,
                Discount = a.DiscountPrice,
                Category = a.Category,
                IsActive = a.IsActive,
                Stock = a.Stock,
                CreatAt = a.CreatedAt,
                CoverURL = a.CoverURL
            }).ToListAsync();

            return View(vm);
        }

        // GET: Mall
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Merchandise.Include(m => m.GameDetail);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: Mall/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise
                .Include(m => m.GameDetail)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // GET: Mall/Create
        public IActionResult Create()
        {
            ViewData["GameDetailId"] = new SelectList(_context.GameDetails, "Id", "Artist");
            return View();
        }

        // POST: Mall/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameDetailId,CoverURL,ImageGalleryJson,Description,des,Brand,Price,DiscountPrice,Stock,Category,IsActive,CreatedAt,UpdatedAt")] Merchandise merchandise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(merchandise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameDetailId"] = new SelectList(_context.GameDetails, "Id", "Artist", merchandise.GameDetailId);
            return View(merchandise);
        }

        // GET: Mall/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise.FindAsync(id);
            if (merchandise == null)
            {
                return NotFound();
            }
            ViewData["GameDetailId"] = new SelectList(_context.GameDetails, "Id", "Artist", merchandise.GameDetailId);
            return View(merchandise);
        }

        // POST: Mall/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameDetailId,CoverURL,ImageGalleryJson,Description,des,Brand,Price,DiscountPrice,Stock,Category,IsActive,CreatedAt,UpdatedAt")] Merchandise merchandise)
        {
            if (id != merchandise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(merchandise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MerchandiseExists(merchandise.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameDetailId"] = new SelectList(_context.GameDetails, "Id", "Artist", merchandise.GameDetailId);
            return View(merchandise);
        }

        // GET: Mall/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise
                .Include(m => m.GameDetail)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // POST: Mall/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var merchandise = await _context.Merchandise.FindAsync(id);
            if (merchandise != null)
            {
                _context.Merchandise.Remove(merchandise);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MerchandiseExists(int id)
        {
            return _context.Merchandise.Any(e => e.Id == id);
        }
    }
}
