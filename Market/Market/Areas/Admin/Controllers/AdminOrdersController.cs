using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Market.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly MarketContext _context;
        public INotyfService _notyfService { get; }

        public AdminOrdersController(MarketContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
		private int pageSize = 3;
		public IActionResult Index()
        {    
            var orders = (IQueryable<Order>)_context.Orders.Include(o => o.Employee)
                .OrderBy(x => x.OrderDate);
			// Tính số trang
			int pageNum = (int)Math.Ceiling(orders.Count() / (float)pageSize);
			// Trả về số trang hiển thị trong nav-trang
			ViewBag.pageNum = pageNum;
			// Lấy dữ liệu trang đầu
			var result = orders.Take(pageSize).ToList();

			List<SelectListItem> listTrangThai = new List<SelectListItem>();
            listTrangThai.Add(new SelectListItem() { Text = "Chờ xác nhận", Value = "1" });
            listTrangThai.Add(new SelectListItem() { Text = "Đã xác nhận", Value = "2" });
            listTrangThai.Add(new SelectListItem() { Text = "Đã hủy đơn", Value = "3" });
            listTrangThai.Add(new SelectListItem() { Text = "Đã giao", Value = "4" });
            ViewData["TrangThaiDonHang"] = listTrangThai;

            return View(result);
        }
		public IActionResult FilterOrder(int? status, string keyword, int? pageIndex)
		{
			var orders = (IQueryable<Order>)_context.Orders.Include(o=>o.Employee)
				                     .AsNoTracking()
                                     .OrderBy(x => x.OrderDate);
			int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);
			if (status != null && status>0)
			{
				orders = orders.Where(m => m.Status == status);
				ViewBag.status = status;
			}
			if (keyword != null)
			{
                orders = (IOrderedQueryable<Order>)orders.Where(x => 
                                         x.OrderId.Contains(keyword) ||
										 x.EmployeeId.Contains(keyword) ||
										 x.CustomerName.Contains(keyword) ||
										 x.Address.Contains(keyword) ||
										 x.Phone.Contains(keyword));
				ViewBag.keyword = keyword;
			}
			int pageNum = (int)Math.Ceiling(orders.Count() / (float)pageSize);
			ViewBag.pageNum = pageNum;
			var result = orders
				.Skip(pageSize * (page - 1))
				.Take(pageSize)
				.Include(m => m.Employee)
                .OrderBy(m=>m.OrderDate);
			return PartialView("OrdersPaging", result);
		}

		public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
            .Include(o => o.Employee)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDetails = order.OrderDetails.ToList();

            var productNames = orderDetails.Select(od => od.Product.ProductName).ToList();

            ViewData["ProductNames"] = productNames;
            ViewBag.Details = orderDetails;
            return View(order);
        }
		public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,EmployeeId,Status,OrderDate,TotalOrders,CustomerName,Address,Phone,Note")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
				_notyfService.Success("Thêm mới thành công");
				return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            return View(order);
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            List<SelectListItem> listTrangThai = new List<SelectListItem>();
            listTrangThai.Add(new SelectListItem() { Text = "Chờ xác nhận", Value = "1" });
            listTrangThai.Add(new SelectListItem() { Text = "Đã xác nhận", Value = "2" });
            listTrangThai.Add(new SelectListItem() { Text = "Đã hủy đơn", Value = "3" });
            listTrangThai.Add(new SelectListItem() { Text = "Đã giao", Value = "4" });
            ViewData["TrangThaiDonHang"] = listTrangThai;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderId,EmployeeId,Status,OrderDate,TotalOrders,CustomerName,Address,Phone,Note")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
					_notyfService.Success("Cập nhật thành công");
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            
            return View(order);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'MarketContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
			_notyfService.Success("Hủy hóa đơn thành công");
			return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(string id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
