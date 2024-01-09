using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Market.Models;

using Microsoft.Data.SqlClient;
using System.Web.Mvc;
using System.Security.Cryptography;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        private readonly MarketContext _context;

        public EmployeesController(MarketContext context)
        {
            _context = context;
        }

        // GET: Employees
       
        // GET: Employees/Search

        private int pageSize = 3;
        
        public IActionResult Index(string? keyword)
        {
            var employees = _context.Employees.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                     
                keyword = keyword.ToLower();
                employees = employees.Where(e =>
                    e.EmployeeName.Contains(keyword) ||
                    e.EmployeeId.Contains(keyword) ||
                    (e.Position != null && e.Position.Contains(keyword))    
        );            
                ViewBag.keyword = keyword;

            }
                int pageNum = (int)Math.Ceiling(employees.Count() / (float)pageSize);
                //trả số trang về view để hiển thị nav-trang
                ViewBag.pageNum = pageNum;
                //Lấy dữ liệu trang đầu
                var result = employees.Take(pageSize).ToList();

            // Kiểm tra xem có thông báo thành công trong TempData
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(result);
                       
        }

      
        public IActionResult EmployeeFilter(string? keyword, int? pageIndex)
        {
            //Lấy toàn bộ learners trong dbset chuyển về IQueryable<Learner> để query 
            var employees = _context.Employees.AsQueryable();
            //lấy chỉ số trang, nếu chỉ số trang null thì gán ngầm định bằng 1
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);          
            //nếu có keyword thì tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                //tìm kiếm
                /*employees = employees.Where(l => l.EmployeeName.ToLower().Contains(keyword.ToLower()));*/
                keyword = keyword.ToLower();
                employees = employees.Where(e =>
                    e.EmployeeName.Contains(keyword) ||
                    e.EmployeeId.Contains(keyword) ||
                    (e.Position != null && e.Position.Contains(keyword))

        );
                ViewBag.keyword = keyword;
            }
            //tính số trang
            int pageNum = (int)Math.Ceiling(employees.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            //Chonj duwx lieuej trong trang hiện tại 
            var result = employees.Skip(pageSize * (page - 1)).Take(pageSize);
            return PartialView("LearnerTable", result);
        }




        // GET: Employees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeName,PhoneNumber,Position,Status")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
*/     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeName,PhoneNumber,Position,Status")] Employee employee)
        {
            // Kiểm tra xem mã nhân viên đã tồn tại hay chưa
            if (_context.Employees.Any(e => e.EmployeeId == employee.EmployeeId))
            {
                ModelState.AddModelError("EmployeeId", "Mã nhân viên đã tồn tại.");
                return View(employee);
            }

            // Nếu mã nhân viên chưa tồn tại và ModelState hợp lệ, thêm mới
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();

                // Đặt thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm nhân viên thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmployeeId,EmployeeName,PhoneNumber,Position,Status")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'MarketContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }


    }
}
