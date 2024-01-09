using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Market.Models;



namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly MarketContext _context;

        public UsersController(MarketContext context)
        {
            _context = context;
        }
          

        private int pageSize = 3;
        public IActionResult Index(string? keyword)
        {
            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                users = users.Where(e =>
                    e.UserName.Contains(keyword) ||
                    e.EmployeeId.Contains(keyword)
             );
                ViewBag.keyword = keyword;

            }
            int pageNum = (int)Math.Ceiling(users.Count() / (float)pageSize);
            //trả số trang về view để hiển thị nav-trang
            ViewBag.pageNum = pageNum;
            //Lấy dữ liệu trang đầu
            var result = users.Take(pageSize).ToList();

            // Kiểm tra xem có thông báo thành công trong TempData
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(result);

        }

        public IActionResult UserFilter(string? keyword, int? pageIndex)
        {
            //Lấy toàn bộ learners trong dbset chuyển về IQueryable<Learner> để query 
            var users = _context.Users.AsQueryable();
            //lấy chỉ số trang, nếu chỉ số trang null thì gán ngầm định bằng 1
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);
            //nếu có keyword thì tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                users = users.Where(e =>
                    e.UserName.Contains(keyword) ||
                    e.EmployeeId.Contains(keyword)
             );
                ViewBag.keyword = keyword;
            }
            //tính số trang
            int pageNum = (int)Math.Ceiling(users.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            //Chonj duwx lieuej trong trang hiện tại 
            var result = users.Skip(pageSize * (page - 1)).Take(pageSize);
            return PartialView("UserTable", result);
        }
        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /* [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> Create([Bind("UserName,Password,EmployeeId,Status")] User user)
         {
             if (ModelState.IsValid)
             {
                 _context.Add(user);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
             }
             ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", user.EmployeeId);
             return View(user);
         }

 */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Password,EmployeeId,Status")] User user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem mã nhân viên đã có trong bảng User hay chưa
                var existingUser = _context.Users.FirstOrDefault(u => u.EmployeeId == user.EmployeeId);

                if (existingUser != null)
                {
                    // Mã nhân viên đã tồn tại trong bảng User, thêm lỗi vào ModelState
                    ModelState.AddModelError("EmployeeId", "Mã nhân viên đã có tài khoản.");                   
                    return View(user);
                }
                else
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    // Thông báo thành công
                    TempData["SuccessMessage"] = "Tài khoản đã được tạo thành công.";
                    return RedirectToAction(nameof(Index));
                }
            }

/*            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", user.EmployeeId);
*/            return View(user);
        }




        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", user.EmployeeId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserName,Password,EmployeeId,Status")] User user)
        {
            if (id != user.UserName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserName))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", user.EmployeeId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(m => m.UserName == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'MarketContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.UserName == id)).GetValueOrDefault();
        }
    }
}
