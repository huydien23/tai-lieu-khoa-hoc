using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiLieuKhoaHoc.Web.Models;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;

        public AccountController(UserManager<NguoiDung> userManager, SignInManager<NguoiDung> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đăng nhập";
                return View();
            }

            // Tìm user bằng username hoặc email
            var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {user.HoTen ?? user.UserName}";

                    // Redirect based on role
                    if (user.VaiTro == VaiTroNguoiDung.QuanTriVien)
                        return RedirectToAction("Dashboard", "Admin");
                    else if (user.VaiTro == VaiTroNguoiDung.GiangVien)
                        return RedirectToAction("Dashboard", "Lecturer");
                    else
                        return RedirectToAction("Dashboard", "Student");
                }
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
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword, string role = "student")
        {
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

            // Tạo user mới
            var user = new NguoiDung
            {
                UserName = username,
                Email = email,
                HoTen = username, // Tạm thời dùng username làm họ tên
                VaiTro = role switch
                {
                    "admin" => VaiTroNguoiDung.QuanTriVien,
                    "lecturer" => VaiTroNguoiDung.GiangVien,
                    _ => VaiTroNguoiDung.SinhVien
                },
                EmailConfirmed = true,
                TrangThaiHoatDong = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
                break; // Chỉ hiển thị lỗi đầu tiên
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["InfoMessage"] = "Đã đăng xuất thành công";
            return RedirectToAction("Index", "Home");
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

        public async Task<IActionResult> Profile()
        {
            ViewData["Title"] = "Thông tin Cá nhân";

            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.User = user;
            return View();
        }

        public IActionResult ChangePassword()
        {
            ViewData["Title"] = "Đổi Mật khẩu";

            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận không khớp";
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                await _signInManager.RefreshSignInAsync(user);
                return View();
            }

            foreach (var error in result.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
                break;
            }

            return View();
        }
    }
}
