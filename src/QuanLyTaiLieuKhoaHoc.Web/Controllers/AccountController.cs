using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            // TODO: Implement actual authentication logic
            // For now, just redirect based on role for demo purposes

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đăng nhập";
                return View();
            }

            // Demo authentication - replace with real logic
            if ((username == "thuthu" && password == "123456" && role == "admin") ||
                (username == "giangvien" && password == "123456" && role == "lecturer") ||
                (username == "sinhvien" && password == "123456" && role == "student"))
            {
                // Set user session/claims here
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("Username", username);

                TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {username}";

                // Redirect based on role
                return role switch
                {
                    "admin" => RedirectToAction("Dashboard", "Admin"),
                    "lecturer" => RedirectToAction("Dashboard", "Lecturer"),
                    "student" => RedirectToAction("Dashboard", "Student"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            TempData["ErrorMessage"] = "Thông tin đăng nhập không chính xác";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "Đã đăng xuất thành công";
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
