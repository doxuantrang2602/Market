using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Market.Models;
using X.PagedList;
using Market.Areas.Admin.Helpers;
using System.Security.Cryptography;

namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly MarketContext _context;

        public CategoriesController(MarketContext context)
        {
            _context = context;
        }


        // GET: Admin/Categories
        private int pageSize = 3;
        [Route("Admin/Categories/Index")]
        public async Task<IActionResult> Index(int? s)
        {
            var categories = (IQueryable<Category>)_context.Categories;

            if (s != null)
            {
                categories = (IQueryable<Category>)_context.Categories.Where(c => c.Status == s);
            }
            // tính số trang 
            int pageNum = (int)Math.Ceiling(categories.Count() / (float)pageSize);

            // trả số trang về view để hiển thị nav-trang
            ViewBag.pageNum = pageNum;

            //lấy dữ liệu trang đầu
            var result = categories.Take(pageSize).ToList();

            List<SelectListItem> listStatus = CategoryStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;

            return View(result);

        }
        public IActionResult CategoryFilter(int? s, string? keyword, int? pageIndex)
        {
            var categories = (IQueryable<Category>)_context.Categories;

            // lấy chỉ số trang
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);

            if (s != null && s != 0)
            {
                //lọc
                categories = categories.Where(c => c.Status == s);

                // gửi s về view để ghi lại trên nav-phân trang
                ViewBag.s = s;
            }

            if (keyword != null && keyword != "")
            {
                categories = categories.Where(x => x.CategoryId.ToLower().Contains(keyword.ToLower()) ||
                                                x.CategoryName.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;
            }

            // tính số trang 
            int pageNum = (int)Math.Ceiling(categories.Count() / (float)pageSize);

            // gửi về view
            ViewBag.pageNum = pageNum;

            // chọn dữ liệu trong trang hiện tại 
            var result = categories.Skip(pageSize * (page - 1)).Take(pageSize);

            return PartialView("CategoryTable", result);
        }
        /*
        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        */
        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            List<SelectListItem> listStatus = CategoryStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Status")] Category category)
        {
            if (_context.Categories.Any(c => c.CategoryId == category.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Mã danh mục đã tồn tại.");

                List<SelectListItem> status = CategoryStatusHelper.GetStatusList();
                ViewBag.Status = status;
                return View(category);
            }
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<SelectListItem> listStatus = CategoryStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            List<SelectListItem> listStatus = CategoryStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CategoryId,CategoryName,Status")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            List<SelectListItem> listStatus = CategoryStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'MarketContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(string id)
        {
            return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
