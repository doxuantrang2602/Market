using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Market.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.CodeAnalysis;
using PayPal.Api;

namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrderDetailsController : Controller
    {
        private readonly MarketContext _context;
        public INotyfService _notyfService { get; }

        public AdminOrderDetailsController(MarketContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .Where(od => od.OrderId == id)
                .ToListAsync();
            if (orderDetail == null)
            {               
                ViewBag.Order = order;
                return View();
            }
            ViewBag.Order = order;
            return View(orderDetail);
        }
        public IActionResult Create(string orderID)
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderID);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            var orderDetail = new OrderDetail();
            if (!string.IsNullOrEmpty(orderID))
            {
                orderDetail.OrderId = orderID;
            }

            return View(orderDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,Price,Amount,Sale,Total")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm sản phẩm thành công !");
                return RedirectToAction("Index", new { id = orderDetail.OrderId });
                
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", orderDetail.ProductId);

            return View(orderDetail);
        }

        public async Task<IActionResult> Edit(string orderID, string productID)
        {
            if (orderID == null || productID == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.OrderId == orderID && m.ProductId == productID);

            if (orderDetail == null)
            {
                return NotFound();
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", orderDetail.ProductId);

            return View(orderDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string orderID, string productID, [Bind("OrderId,ProductId,Price,Amount,Sale,Total")] OrderDetail orderDetail)
        {
            if (orderID != orderDetail.OrderId || productID != orderDetail.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();

                    _notyfService.Success("Cập nhật thành công");
                    return RedirectToAction("Index", new { id = orderID });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!OrderDetailExists(orderDetail.OrderId, orderDetail.ProductId))
                    {
                        return NotFound();
                    }
                }
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", orderDetail.ProductId);
            return View(orderDetail);
        }

        public async Task<IActionResult> Delete(string orderID, string productID)
        {
            if (orderID == null || productID == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

			var orderDetail = await _context.OrderDetails
		        .Include(o => o.Order)
		        .Include(o => o.Product)
		        .FirstOrDefaultAsync(m => m.OrderId == orderID && m.ProductId == productID);
			if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string orderID, string productID)
        {
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'MarketContext.OrderDetails'  is null.");
            }
			var orderDetail = await _context.OrderDetails
		     .FirstOrDefaultAsync(m => m.OrderId == orderID && m.ProductId == productID); 
			if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
				_notyfService.Success("Xóa sản phẩm thành công !");
				return RedirectToAction("Index", new { id = orderID });
            }

            return NotFound();
        }

        private bool OrderDetailExists(string orderId, string productId)
        {
            return (_context.OrderDetails?.Any(e => e.OrderId == orderId && e.ProductId == productId)).GetValueOrDefault();
        }
    }
}
