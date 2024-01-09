using AspNetCoreHero.ToastNotification.Abstractions;
using Market.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {     
        private readonly MarketContext _context;
        public SearchController(MarketContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FindOrder(string keyword)
        {
            List<Order> listOrder = new List<Order>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                listOrder = _context.Orders.AsNoTracking()
                                     .Include(o => o.Employee)
                                    .Take(10)
                                    .ToList();
                if (listOrder == null)
                {
                    return PartialView("OrdersTable", null);
                }
                else
                {
                    return PartialView("OrdersTable", listOrder);
                }
            }
            listOrder = _context.Orders.AsNoTracking()
                                  .Include(o => o.Employee)
                                  .Where(x => x.OrderId.Contains(keyword) || 
                                         x.EmployeeId.Contains(keyword) ||
                                         x.CustomerName.Contains(keyword) ||
                                         x.Address.Contains(keyword) ||
                                         x.Phone.Contains(keyword))
                                  .OrderBy(x => x.OrderDate)
                                  .Take(10)
                                  .ToList();
            if (listOrder == null)
            {
                return PartialView("OrdersTable", null);
            }
            else
            {
                return PartialView("OrdersTable", listOrder);
            }
        }
		public IActionResult FilterOrderByStatus(int? TrangThai)
		{  
			var Orders = _context.Orders.Include(o => o.Employee)
			.AsNoTracking()
			.OrderBy(x => x.OrderDate);
			if (TrangThai != null && TrangThai > 0)
			{
				Orders = (IOrderedQueryable<Order>)Orders.Where(x => x.Status == TrangThai);
			}
			var searchOrder = Orders.ToList();
			return PartialView("OrdersTable", searchOrder);
		}
		public IActionResult GetPage(int page, int pageSize)
		{
			var totalRecords = _context.Orders.Include(o => o.Employee).Count();
			var pageCount = (int)Math.Ceiling((double)totalRecords / pageSize);
			if (page < 1)
			{
				page = 1;
			}
			else if (page > pageCount)
			{
				page = pageCount;
			}
			var orders = _context.Orders
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();
			return PartialView("OrdersPaging", orders);
		}

		public IActionResult FindProduct(string keyword)
		{
			List<Product> listProducts = new List<Product>();
			if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
			{
				listProducts = _context.Products.Include(p => p.Category).Include(p => p.Supplier).AsNoTracking().Take(10).ToList();
				if (listProducts == null)
				{
					return PartialView("ProductsTable", null);
				}
				else
				{
					return PartialView("ProductsTable", listProducts);
				}
			}
			listProducts = _context.Products.Include(p => p.Category).Include(p => p.Supplier).AsNoTracking()
								.Where(x => x.ProductId.Contains(keyword) ||
										x.ProductName.Contains(keyword) ||
										x.Unit.Contains(keyword))
								.OrderBy(x => x.ProductId)
								.Take(10)
								.ToList();
			if (listProducts == null)
			{
				return PartialView("ProductsTable", null);
			}
			else
			{
				return PartialView("ProductsTable", listProducts);
			}
		}
		public IActionResult FilterProductByStatus(int? statusSearch)
		{
			var listProducts = _context.Products.Include(p => p.Category).Include(p => p.Supplier).AsNoTracking()
								.OrderBy(x => x.ProductId);
			if (statusSearch != null && statusSearch > 0)
			{
				listProducts = (IOrderedQueryable<Product>)listProducts.Where(x => x.Status == statusSearch);
			}
			var searchProduct = listProducts.ToList();
			return PartialView("ProductsTable", searchProduct);
		}
		public IActionResult FilterProductByCategory(string? category)
		{
			var listProducts = _context.Products.Include(p => p.Category).Include(p => p.Supplier).AsNoTracking()
								.OrderBy(x => x.ProductId);
			if (category != null && category.Length > 0)
			{
				listProducts = (IOrderedQueryable<Product>)listProducts.Where(x => x.CategoryId == category);
			}
			var searchProduct = listProducts.ToList();
			return PartialView("ProductsTable", searchProduct);
		}

	}
}
