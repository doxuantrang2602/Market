using Market.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Market.Controllers
{
    public class HomeController : Controller
    {
        private MarketContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,MarketContext context)
        {
            _logger = logger;
            db = context;
        }
        private int pageSize = 1;

        public IActionResult Index(string? mid)
        {
            var product = (IQueryable<Product>)db.Products;
            if(mid != null)
            {
                product = (IQueryable<Product>)db.Products.Where(m => m.CategoryId == mid).Include(m => m.Category);
            }
            int pageNum = (int)Math.Ceiling(product.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            var result = product.Take(pageSize).ToList();
            return View(result);
        }

        public IActionResult Filter(int? pageIndex, string? keyword)
        {
            var product = (IQueryable<Product>) db.Products.Include(m => m.Category);
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);
            if(keyword != null)
            {
                product = product.Where(l => l.CategoryId.ToLower().Contains(keyword.ToLower()));
            }
            int pageNum = (int)Math.Ceiling(product.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            var result = product.Skip(pageSize * (page - 1)).Take(pageSize);
            return PartialView("Search",result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}