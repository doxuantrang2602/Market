using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Market.Models;
using Market.Areas.Admin.Helpers;

namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SuppliersController : Controller
    {
        private readonly MarketContext _context;

        public SuppliersController(MarketContext context)
        {
            _context = context;
        }

        // GET: Admin/Suppliers
        private int pageSize = 3;
        [Route("Admin/Suppliers/Index")]
        public async Task<IActionResult> Index(int? s)
        {
            var suppliers = (IQueryable<Supplier>)_context.Suppliers;

            if (s != null)
            {
                suppliers = (IQueryable<Supplier>)_context.Suppliers.Where(c => c.Status == s);
            }
            // tính số trang 
            int pageNum = (int)Math.Ceiling(suppliers.Count() / (float)pageSize);

            // trả số trang về view để hiển thị nav-trang
            ViewBag.pageNum = pageNum;

            //lấy dữ liệu trang đầu
            var result = suppliers.Take(pageSize).ToList();

            List<SelectListItem> listStatus = CategoryStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;

            return View(result);

        }


        public IActionResult SupplierFilter(int? s, string? keyword, int? pageIndex)
        {
            var suppliers = (IQueryable<Supplier>)_context.Suppliers;

            // lấy chỉ số trang
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);

            if (s != null && s != 0)
            {
                //lọc
                suppliers = suppliers.Where(c => c.Status == s);

                // gửi s về view để ghi lại trên nav-phân trang
                ViewBag.s = s;
            }

            if (keyword != null && keyword != "")
            {
                suppliers = suppliers.Where(x => x.SupplierId.ToLower().Contains(keyword.ToLower()) ||
                                                x.SupplierName.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;
            }

            // tính số trang 
            int pageNum = (int)Math.Ceiling(suppliers.Count() / (float)pageSize);

            // gửi về view
            ViewBag.pageNum = pageNum;

            // chọn dữ liệu trong trang hiện tại 
            var result = suppliers.Skip(pageSize * (page - 1)).Take(pageSize);

            return PartialView("SupplierTable", result);
        } // GET: Admin/Suppliers/Details/5


        // GET: Admin/Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,SupplierName,Address,Phone,Status")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Admin/Suppliers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Admin/Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SupplierId,SupplierName,Address,Phone,Status")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
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
            return View(supplier);
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }
        // GET: Admin/Suppliers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Admin/Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'MarketContext.Suppliers'  is null.");
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(string id)
        {
            return (_context.Suppliers?.Any(e => e.SupplierId == id)).GetValueOrDefault();
        }
    }
}
