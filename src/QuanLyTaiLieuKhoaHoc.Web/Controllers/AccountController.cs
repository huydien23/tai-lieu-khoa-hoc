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
            ViewData["Title"] = "Đăng ký Tài khoản";
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword, string role = "student")
        {
            // TODO: Implement registration logic
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin";
                return View();
            }

            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp";
                return View();
            }

            // Demo: Always success for now
            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "Đã đăng xuất thành công";
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Quên Mật khẩu";
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            // TODO: Implement forgot password logic
            TempData["SuccessMessage"] = "Hướng dẫn đặt lại mật khẩu đã được gửi tới email của bạn";
            return View();
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "Thông tin Cá nhân";
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }
            ViewBag.Username = username;
            return View();
        }

        public IActionResult ChangePassword()
        {
            ViewData["Title"] = "Đổi Mật khẩu";
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // TODO: Implement change password logic
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận không khớp";
                return View();
            }

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return View();
        }
    }
}
