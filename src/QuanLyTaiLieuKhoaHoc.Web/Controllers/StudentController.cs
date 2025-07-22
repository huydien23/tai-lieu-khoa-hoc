using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Data;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [ActionName("Dashboard-Student")]
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard Sinh viên";
            ViewData["UserRole"] = "student";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy thông tin người dùng
            var currentUser = await _context.Users.Include(u => u.ChuyenNganh).FirstOrDefaultAsync(u => u.Id == userId);
            ViewBag.CurrentUser = currentUser;

            // Yêu thích
            var danhSachYeuThich = await _context.YeuThichTaiLieu
                .Include(y => y.TaiLieu)
                .Where(y => y.UserId == userId && y.TaiLieu != null)
                .ToListAsync();

            // Mượn trả
            var danhSachPhieu = await _context.PhieuMuonTra
                .Where(p => p.MaNguoiMuon == userId)
                .ToListAsync();
            
            var model = new DashboardViewModel
            {
                TaiLieuYeuThich = danhSachYeuThich,
                SoPhieuDangMuon = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.DaDuyet && p.NgayTra == null),
                SoPhieuChoDuyet = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.ChoDuyet),
                SoPhieuDaTra = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.DaTra),
                SoPhieuTuChoi = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.TuChoi)
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

        public IActionResult TimKiemTaiLieu()
        {
            ViewData["Title"] = "Tìm kiếm Tài liệu";
            return View();
        }

        public IActionResult XemTaiLieu()
        {
            ViewData["Title"] = "Xem Tài liệu";
            return View();
        }
    }
}
