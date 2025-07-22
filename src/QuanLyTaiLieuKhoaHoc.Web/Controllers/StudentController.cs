using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .Include(p => p.TaiLieu)
                .Where(p => p.MaNguoiMuon == userId)
                .ToListAsync();
            
            // Lấy danh sách tài liệu đang mượn (phiếu DaDuyet và chưa trả của sinh viên)
            var taiLieuDangMuon = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Where(p => p.MaNguoiMuon == userId && p.TrangThai == TrangThaiPhieu.DaDuyet && p.NgayTra == null)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();

            // Lịch sử tải xuống
            var lichSuTaiXuong = await _context.LichSuTaiTaiLieu
                .Include(l => l.TaiLieu)
                .Where(l => l.MaNguoiDung == userId)
                .OrderByDescending(l => l.ThoiGianTai)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TaiLieuYeuThich = danhSachYeuThich,
                SoPhieuDangMuon = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.DaDuyet && p.NgayTra == null),
                SoPhieuChoDuyet = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.ChoDuyet),
                SoPhieuDaTra = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.DaTra),
                SoPhieuTuChoi = danhSachPhieu.Count(p => p.TrangThai == TrangThaiPhieu.TuChoi),
                TaiLieuDangMuon = taiLieuDangMuon,
                PhieuMuonTra = danhSachPhieu, // Truyền danh sách phiếu mượn cho tab động
                SoLuotTaiXuong = lichSuTaiXuong.Count,
                LichSuTaiXuong = lichSuTaiXuong
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

        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> ManageBorrowReturn()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var list = await _context.PhieuMuonTra
                .Include(p => p.TaiLieu)
                .Where(p => p.MaNguoiMuon == user.Id)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();
            return View(list);
        }

        [HttpPost]
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> CancelRequest(int maPhieu)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var phieu = await _context.PhieuMuonTra.FirstOrDefaultAsync(p => p.MaPhieu == maPhieu && p.MaNguoiMuon == user.Id);
            if (phieu == null || phieu.TrangThai != TrangThaiPhieu.ChoDuyet)
            {
                return Json(new { success = false, message = "Không thể hủy yêu cầu này!" });
            }
            phieu.TrangThai = TrangThaiPhieu.TuChoi;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã hủy yêu cầu mượn!" });
        }
    }
}
