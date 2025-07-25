using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    [Authorize(Roles = "GiangVien")]
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public LecturerController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [ActionName("Dashboard-Lecturer")]
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard Giảng viên";
            ViewData["UserRole"] = "lecturer";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Forbid();

            // Lấy thông tin người dùng
            var currentUser = await _context.Users.Include(u => u.ChuyenNganh).FirstOrDefaultAsync(u => u.Id == userId);
            ViewBag.CurrentUser = currentUser;

            var ngayHienTai = DateTime.Today;
            var dauTuanNay = ngayHienTai.AddDays(-(int)ngayHienTai.DayOfWeek + 1);
            var dauTuanTruoc = dauTuanNay.AddDays(-7);
            var cuoiTuanTruoc = dauTuanNay.AddDays(-1);

            // Lượt tải của người dùng hiện tại
            var tongLuotTai = await _context.LichSuTaiTaiLieu
                .Where(l => l.MaNguoiDung == userId)
                .CountAsync();

            var luotTaiTuanNay = await _context.LichSuTaiTaiLieu
                .Where(l => l.MaNguoiDung == userId && l.ThoiGianTai >= dauTuanNay)
                .CountAsync();

            var luotTaiTuanTruoc = await _context.LichSuTaiTaiLieu
                .Where(l => l.MaNguoiDung == userId && l.ThoiGianTai >= dauTuanTruoc && l.ThoiGianTai <= cuoiTuanTruoc)
                .CountAsync();

            // Tài liệu yêu thích
            var taiLieuYeuThich = await _context.YeuThichTaiLieu
                .Include(y => y.TaiLieu)
                .Where(y => y.UserId == userId && y.TaiLieu != null)
                .ToListAsync();

            // Tài liệu mới trong 30 ngày
            var taiLieuMoiTrongThang = await _context.TaiLieu
                .Where(t => t.TrangThai == TrangThaiTaiLieu.DaDuyet && t.NgayTaiLen >= ngayHienTai.AddDays(-30))
                .CountAsync();

            // Tài liệu đã tải gần đây (hiển thị danh sách)
            var taiLieuGanDay = await _context.LichSuTaiTaiLieu
                .Include(l => l.TaiLieu)
                .Where(l => l.MaNguoiDung == userId && l.TaiLieu != null && l.ThoiGianTai >= ngayHienTai.AddDays(-7))
                .OrderByDescending(l => l.ThoiGianTai)
                .Select(l => l.TaiLieu!)
                .Distinct()
                .Take(6)
                .ToListAsync();

            var danhSachTaiLieuXem = taiLieuGanDay.Select(t => new TaiLieuViewModel
            {
                MaTaiLieu = t.MaTaiLieu,
                TenTaiLieu = t.TenTaiLieu,
                TacGia = t.TacGia,
                LoaiFile = t.LoaiFile,
                KichThuocFile = t.KichThuocFile,
                NgayTaiLen = t.NgayTaiLen
            }).ToList();

            // Thống kê trong các tài liệu đã tải gần đây
            var thongKeTheoChuyenNganh = taiLieuGanDay
                .Where(t => t.ChuyenNganh != null)
                .GroupBy(t => t.ChuyenNganh!.TenChuyenNganh)
                .ToDictionary(g => g.Key, g => g.Count());

            var thongKeTheoLoaiTaiLieu = taiLieuGanDay
                .Where(t => t.LoaiTaiLieu != null)
                .GroupBy(t => t.LoaiTaiLieu!.TenLoaiTaiLieu)
                .ToDictionary(g => g.Key, g => g.Count());

            // Gán dữ liệu vào ViewModel
            var model = new DashboardViewModel
            {
                TaiLieuYeuThich = taiLieuYeuThich,
                TaiLieuMoiTrongThang = taiLieuMoiTrongThang,
                TongSoLuotTai = tongLuotTai,
                LuotTaiTrongTuan = luotTaiTuanNay,
                LuotTaiTuanTruoc = luotTaiTuanTruoc,
                SoLuotXemGanDay = danhSachTaiLieuXem.Count,
                TaiLieuXemGanDay = danhSachTaiLieuXem,
                ThongKeTheoChuyenNganh = thongKeTheoChuyenNganh,
                ThongKeTheoLoaiTaiLieu = thongKeTheoLoaiTaiLieu
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.Include(u => u.ChuyenNganh).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string HoTen, string MaSo, string SoDienThoai, string MatKhauHienTai, string MatKhauMoi, string NhapLaiMatKhauMoi)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
                return RedirectToAction("Dashboard-Lecturer");
            }
            bool changed = false;
            if (!string.IsNullOrWhiteSpace(HoTen) && HoTen != user.HoTen)
            {
                user.HoTen = HoTen;
                changed = true;
            }
            if (!string.IsNullOrWhiteSpace(MaSo) && MaSo != user.MaSo)
            {
                user.MaSo = MaSo;
                changed = true;
            }
            if (!string.IsNullOrWhiteSpace(SoDienThoai) && SoDienThoai != user.SoDienThoai)
            {
                user.SoDienThoai = SoDienThoai;
                changed = true;
            }
            if (changed)
            {
                await _userManager.UpdateAsync(user);
            }
            // Đổi mật khẩu nếu có nhập
            if (!string.IsNullOrWhiteSpace(MatKhauHienTai) && !string.IsNullOrWhiteSpace(MatKhauMoi) && MatKhauMoi == NhapLaiMatKhauMoi)
            {
                var result = await _userManager.ChangePasswordAsync(user, MatKhauHienTai, MatKhauMoi);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
                    return RedirectToAction("Dashboard-Lecturer");
                }
            }
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Dashboard-Lecturer");
        }

        public IActionResult TimKiemTaiLieu()
        {
            ViewData["Title"] = "Tìm kiếm Tài liệu";
            return View();
        }

        public IActionResult XemThongKe()
        {
            ViewData["Title"] = "Xem Thống kê";
            return View();
        }
    }
}
