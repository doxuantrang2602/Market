using Microsoft.AspNetCore.Mvc;
using Market.Models;
namespace Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccessController : Controller
    {
        MarketContext db = new MarketContext();
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                //  return RedirectToAction("Index","Employees");
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            }

        }
        [HttpPost]   
        public IActionResult Login(User user)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                // Xác minh tên đăng nhập và mật khẩu
                var u = db.Users
                    .Where(x => x.UserName.Equals(user.UserName) && x.Password.Equals(user.Password) && x.Status == 1)
                    .FirstOrDefault();

                if (u != null)
                {
                    HttpContext.Session.SetString("UserName", user.UserName);
                    // Đăng nhập thành công, chuyển hướng đến trang Employees
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    // Trạng thái tài khoản không hợp lệ hoặc tài khoản không tồn tại, hiển thị thông báo lỗi
                    ViewBag.ErrorMessage = "Tài khoản của bạn không hợp lệ hoặc không tồn tại.";
                    return View();
                }
            }
            return View();
        }

    }
}
