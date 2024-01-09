using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Market.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Market.Areas.Admin.Helpers;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;

        private readonly MarketContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(MarketContext context, IWebHostEnvironment webHostEnvironment, ILogger<ProductsController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        public async Task<string> GetImageNameById(string productId)
        {
            // Thực hiện truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm dựa trên ProductId
            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                // Lấy tên tệp ảnh từ đối tượng sản phẩm
                string imageName = product.Img;

                return imageName;
            }

            // Trả về null nếu không tìm thấy sản phẩm với ProductId tương ứng
            return null;
        }


        // GET: Admin/Products

        private int pageSize = 3;

        [Route("Admin/Products/Index")]
        public async Task<IActionResult> Index(int? s, string? cid)
        {
            var products = (IQueryable<Product>)_context.Products.Include(p => p.Category).Include(p => p.Supplier);

            if (s != null)
            {
                products = (IQueryable<Product>)_context.Products.Where(x => x.Status == s).Include(p => p.Category).Include(p => p.Supplier);
            }
            if (cid != null)
            {
                products = (IQueryable<Product>)_context.Products.Where(x => x.CategoryId == cid).Include(p => p.Category).Include(p => p.Supplier);
            }
            // tính số trang 
            int pageNum = (int)Math.Ceiling(products.Count() / (float)pageSize);

            // trả số trang về view để hiển thị nav-trang
            ViewBag.pageNum = pageNum;

            //lấy dữ liệu trang đầu
            var result = products.Take(pageSize).ToList();

            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            List<SelectListItem> listStatus = ProductStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;

            return View(result);
        }
        public IActionResult ProductFilter(int? s, string? cid, string? keyword, int? pageIndex)
        {
            // lấy toàn bộ listproduct trong dbset chuyển về IQueryable<Product> để query
            var products = (IQueryable<Product>)_context.Products;

            // lấy chỉ số trang
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);

            // lọc product nếu có s
            if (s != null && s != 0)
            {
                //lọc
                products = products.Where(x => x.Status == s);

                // gửi s về view để ghi lại trên nav-phân trang
                ViewBag.s = s;
            }
            // lọc theo cid
            if (cid != null && cid != "")
            {
                products = products.Where(x => x.CategoryId == cid);
                ViewBag.cid = cid;
            }
            if (keyword != null && keyword != "")
            {
                products = products.Where(x => x.ProductId.ToLower().Contains(keyword.ToLower()) ||
                                                x.ProductName.ToLower().Contains(keyword.ToLower()) ||
                                                x.Unit.ToLower().Contains(keyword.ToLower()) ||
                                                x.Supplier.SupplierName.ToLower().Contains(keyword.ToLower()));

                ViewBag.keyword = keyword;
            }

            // tính số trang 
            int pageNum = (int)Math.Ceiling(products.Count() / (float)pageSize);

            // gửi về view
            ViewBag.pageNum = pageNum;

            // chọn dữ liệu trong trang hiện tại 
            var result = products.Skip(pageSize * (page - 1)).Take(pageSize).Include(p => p.Category).Include(p => p.Supplier);

            return PartialView("ProductTable", result);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.Where(c => c.Status == 1), "CategoryId", "CategoryName");

            List<SelectListItem> listStatus = ProductStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;

            /*ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Suppliers", "SupplierId");*/
            ViewBag.SupplierId = new SelectList(_context.Suppliers.Where(s => s.Status == 1), "SupplierId", "SupplierName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Unit,Price,Amount,SupplierId,CategoryId,Img,Detail,Status")] Product product, IFormFile imgFile)
        {
            if (_context.Products.Any(p => p.ProductId == product.ProductId))
            {
                ModelState.AddModelError("ProductId", "Mã sản phẩm đã tồn tại.");
                List<SelectListItem> lstatus = ProductStatusHelper.GetStatusList();
                ViewBag.Status = lstatus;
                return View(product);
            }
            if (ModelState.IsValid)
            {
                if (imgFile != null && imgFile.Length > 0)
                {
                    string id = product.ProductId.ToString();
                    string fileName = "";

                    int index = imgFile.FileName.IndexOf('.');
                    fileName = "product" + id + "." + imgFile.FileName.Substring(index + 1);
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "product");
                    string filePath = Path.Combine(uploadFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imgFile.CopyToAsync(fileStream);
                    }
                    product.Img = fileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(_context.Categories.Where(c => c.Status == 1), "CategoryId", "CategoryName");

            List<SelectListItem> listStatus = ProductStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;

            ViewBag.SupplierId = new SelectList(_context.Suppliers.Where(s => s.Status == 1), "SupplierId", "SupplierName");
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_context.Categories.Where(c => c.Status == 1), "CategoryId", "CategoryName");

            ViewBag.SupplierId = new SelectList(_context.Suppliers.Where(s => s.Status == 1), "SupplierId", "SupplierName");

            List<SelectListItem> listStatus = ProductStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProductId,ProductName,Unit,Price,Amount,SupplierId,CategoryId,Img,Detail,Status")] Product product, IFormFile picture)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            /*string oldFileName = product.Img; // Lưu tên file cũ*/

            if (picture != null && picture.Length > 0)
            {
                string imgName = "";
                int index = picture.FileName.LastIndexOf('.');
                imgName = "product" + id + "." + picture.FileName.Substring(index + 1);
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "product");
                string filePath = Path.Combine(uploadFolder, imgName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await picture.CopyToAsync(fileStream);
                }
                product.Img = imgName;
            }
            /*else
            {
                product.Img = oldFileName;
            }*/

            if (ModelState.IsValid)
            {

                try
                {
                    /* product.OnStatusChanged();*/
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            /*else
             {
                *//* ModelState.AddModelError(error.ErrorMessage);
                 return View(product);*//*
                 foreach (var modelState in ModelState.Values)
                 {
                     foreach (var error in modelState.Errors)
                     {
                         _logger.LogError(error.ErrorMessage);
                         ViewBag.ErrorMessage += error.ErrorMessage + " - ";

                     }  
                 }

                 return View(product);
             }*/

            ViewBag.CategoryId = new SelectList(_context.Categories.Where(c => c.Status == 1), "CategoryId", "CategoryName");
            ViewBag.SupplierId = new SelectList(_context.Suppliers.Where(s => s.Status == 1), "SupplierId", "SupplierName");

            List<SelectListItem> listStatus = ProductStatusHelper.GetStatusList();
            ViewBag.Status = listStatus;
            return View(product);
        }

        public string ProcessUpload(IFormFile file)
        {
            return file.ToString();
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'MarketContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(string id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}

