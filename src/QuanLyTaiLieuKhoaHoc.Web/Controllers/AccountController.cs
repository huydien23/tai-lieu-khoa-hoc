using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Models;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;

        public AccountController(
        UserManager<NguoiDung> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<NguoiDung> signInManager)
        {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password, bool RememberMe = false)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đăng nhập";
                return View();
            }

            // Tìm user bằng email hoặc username
            var user = await _userManager.FindByEmailAsync(Email) ?? await _userManager.FindByNameAsync(Email);

            if (user != null)
            {
                // Kiểm tra tài khoản có bị khóa không
                if (!user.TrangThaiHoatDong)
                {
                    TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.";
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user, Password, RememberMe, false);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {user.HoTen ?? user.UserName}";

                    // Redirect based on role
                    if (user.VaiTro == VaiTroNguoiDung.ThuThu)
                        return RedirectToAction("Dashboard-Librarian", "Librarian");
                    else if (user.VaiTro == VaiTroNguoiDung.GiangVien)
                        return RedirectToAction("Dashboard-Lecturer", "Lecturer");
                    else if (user.VaiTro == VaiTroNguoiDung.SinhVien)
                        return RedirectToAction("Dashboard-Student", "Student");
                    else
                        return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    TempData["ErrorMessage"] = "Tài khoản bị khóa do đăng nhập sai nhiều lần. Vui lòng thử lại sau.";
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Mật khẩu không chính xác";
                    return View();
                }
            }

            TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống";
            return View();
        }

        public IActionResult Register()
        {
            ViewData["Title"] = "Đăng ký Tài khoản";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string HoTen, string username, string email, string password, string confirmPassword, string maSo, string soDienThoai, string chuyenNganh, string role = "student")
        {
            if (string.IsNullOrEmpty(HoTen) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin";
                return View();
            }

            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp";
                return View();
            }

            // Kiểm tra email đã tồn tại
            var existingEmailUser = await _userManager.FindByEmailAsync(email);
            if (existingEmailUser != null)
            {
                TempData["ErrorMessage"] = "Email này đã được sử dụng";
                return View();
            }

            // Kiểm tra username đã tồn tại
            var existingUserNameUser = await _userManager.FindByNameAsync(username);
            if (existingUserNameUser != null)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập này đã được sử dụng";
                return View();
            }

            // Lấy MaChuyenNganh từ tên chuyên ngành
            int? maChuyenNganh = null;
            if (!string.IsNullOrEmpty(chuyenNganh))
            {
                using (var scope = HttpContext.RequestServices.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<QuanLyTaiLieuKhoaHoc.Web.Data.ApplicationDbContext>();
                    var chuyenNganhEntity = await context.ChuyenNganh.FirstOrDefaultAsync(c => c.TenChuyenNganh == chuyenNganh);
                    if (chuyenNganhEntity != null)
                    {
                        maChuyenNganh = chuyenNganhEntity.MaChuyenNganh;
                    }
                }
            }

            // Tạo user mới
            var user = new NguoiDung
            {
                UserName = username,
                Email = email,
                HoTen = HoTen, // Sử dụng đúng họ tên từ form
                MaSo = maSo,
                SoDienThoai = soDienThoai,
                VaiTro = role switch
                {
                    "lecturer" => VaiTroNguoiDung.GiangVien,
                    "librarian" => VaiTroNguoiDung.ThuThu,
                    _ => VaiTroNguoiDung.SinhVien
                },
                EmailConfirmed = true,
                TrangThaiHoatDong = true,
                NgayTao = DateTime.Now,
                MaChuyenNganh = maChuyenNganh
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
            // Gán role
            var identityRole = role switch
            {
            "lecturer" => "GiangVien",
            "librarian" => "ThuThu",
            _ => "SinhVien"
            };

            // Tạo role nếu chưa tồn tại
            if (!await _roleManager.RoleExistsAsync(identityRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(identityRole));
            }
 
            // Gán user vào role
            await _userManager.AddToRoleAsync(user, identityRole);

                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            // Hiển thị lỗi từ Identity
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            TempData["ErrorMessage"] = $"Đăng ký thất bại: {errors}";

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
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login");
            }
            // Luôn redirect về trang cài đặt phù hợp với vai trò
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return RedirectToAction("Login");
            switch (user.VaiTro)
            {
                case VaiTroNguoiDung.ThuThu:
                    return RedirectToAction("CaiDat", "Librarian");
                case VaiTroNguoiDung.GiangVien:
                    return RedirectToAction("CaiDat", "Lecturer");
                case VaiTroNguoiDung.SinhVien:
                    return RedirectToAction("CaiDat", "Student");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận không khớp";
                return RedirectToRoleSetting();
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
                return RedirectToRoleSetting();
            }

            foreach (var error in result.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
                break;
            }

            return RedirectToRoleSetting();
        }

        private IActionResult RedirectToRoleSetting()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return RedirectToAction("Login");

            switch (user.VaiTro)
            {
                case VaiTroNguoiDung.ThuThu:
                    return RedirectToAction("CaiDat", "Librarian");
                case VaiTroNguoiDung.GiangVien:
                    return RedirectToAction("CaiDat", "Lecturer");
                case VaiTroNguoiDung.SinhVien:
                    return RedirectToAction("CaiDat", "Student");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ForceSeed()
        {
            try
            {
                // Đảm bảo role 'ThuThu' đã tồn tại
                if (!await _roleManager.RoleExistsAsync("ThuThu"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("ThuThu"));
                }

                var existingUser = await _userManager.FindByEmailAsync("thuthu@library.edu.vn");
                if (existingUser == null)
                {
                    // Tạo thủ thư
                    var librarianUser = new NguoiDung
                    {
                        UserName = "thuthu@library.edu.vn",
                        Email = "thuthu@library.edu.vn",
                        EmailConfirmed = true,
                        HoTen = "Nguyễn Huy Điền",
                        VaiTro = VaiTroNguoiDung.ThuThu,
                        MaChuyenNganh = 1,
                        MaSo = "TT001",
                        NgayTao = DateTime.Now,
                        TrangThaiHoatDong = true
                    };
                    var result = await _userManager.CreateAsync(librarianUser, "thuthu123");

                    if (result.Succeeded)
                    {
                        // Gán role Identity 'ThuThu' cho user
                        await _userManager.AddToRoleAsync(librarianUser, "ThuThu");
                        return Json(new { success = true, message = "Tài khoản thủ thư đã được tạo và gán role thành công!" });
                    }
                    else
                    {
                        return Json(new { success = false, errors = result.Errors });
                    }
                }
                else
                {
                    if (!await _userManager.IsInRoleAsync(existingUser, "ThuThu"))
                    {
                        await _userManager.AddToRoleAsync(existingUser, "ThuThu");
                        return Json(new { success = true, message = "Đã gán role 'ThuThu' cho tài khoản thủ thư!" });
                    }
                    return Json(new { success = false, message = "Tài khoản thủ thư đã tồn tại và đã có role!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = users.Select(u => new
            {
                u.Email,
                u.UserName,
                u.HoTen,
                u.VaiTro,
                u.TrangThaiHoatDong
            }).ToList();

            return Json(new
            {
                TotalUsers = users.Count,
                Users = result
            });
        }
    }
}
