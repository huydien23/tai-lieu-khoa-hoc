using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    [Authorize]
    public class LibrarianController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITaiLieuService _taiLieuService;
        private readonly UserManager<NguoiDung> _userManager;

        public LibrarianController(ApplicationDbContext context, ITaiLieuService taiLieuService, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _taiLieuService = taiLieuService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Kiểm tra quyền Thủ thư
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.VaiTro != VaiTroNguoiDung.ThuThu)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            var model = new DashboardViewModel
            {
                TongSoTaiLieu = await _context.TaiLieu.CountAsync(),
                TongSoNguoiDung = await _context.Users.CountAsync(),
                TongSoLuotTai = await _context.TaiLieu.SumAsync(t => t.LuotTai),
                TaiLieuMoiTrongThang = await _context.TaiLieu
                    .CountAsync(t => t.NgayTaiLen.Month == DateTime.Now.Month
                                && t.NgayTaiLen.Year == DateTime.Now.Year),

                TaiLieuMoiNhat = await _taiLieuService.GetTaiLieuMoiNhatAsync(10),
                TaiLieuPhoBien = await _taiLieuService.GetTaiLieuPhoBienAsync(10),

                ThongKeTheoChuyenNganh = await _context.TaiLieu
                    .Include(t => t.ChuyenNganh)
                    .GroupBy(t => t.ChuyenNganh!.TenChuyenNganh)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),

                ThongKeTheoLoaiTaiLieu = await _context.TaiLieu
                    .Include(t => t.LoaiTaiLieu)
                    .GroupBy(t => t.LoaiTaiLieu!.TenLoaiTaiLieu)
                    .ToDictionaryAsync(g => g.Key, g => g.Count())
            };

            ViewData["Title"] = "Dashboard Thủ thư";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users
                .Include(u => u.ChuyenNganh)
                .OrderBy(u => u.HoTen)
                .ToListAsync();

            ViewData["Title"] = "Quản lý Người dùng";
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDocuments()
        {
            var documents = await _context.TaiLieu
                .Include(t => t.NguoiTaiLen)
                .Include(t => t.ChuyenNganh)
                .Include(t => t.LoaiTaiLieu)
                .OrderByDescending(t => t.NgayTaiLen)
                .ToListAsync();

            ViewData["Title"] = "Quản lý Tài liệu";
            return View(documents);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveDocument(int id)
        {
            var document = await _context.TaiLieu.FindAsync(id);
            if (document != null)
            {
                document.TrangThai = TrangThaiTaiLieu.DaDuyet;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã duyệt tài liệu thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài liệu!";
            }

            return RedirectToAction(nameof(ManageDocuments));
        }

        [HttpPost]
        public async Task<IActionResult> RejectDocument(int id, string? lyDo)
        {
            var document = await _context.TaiLieu.FindAsync(id);
            if (document != null)
            {
                document.TrangThai = TrangThaiTaiLieu.TuChoi;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã từ chối tài liệu!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài liệu!";
            }

            return RedirectToAction(nameof(ManageDocuments));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.TrangThaiHoatDong = !user.TrangThaiHoatDong;
                user.NgayCapNhatCuoi = DateTime.Now;
                await _context.SaveChangesAsync();

                string status = user.TrangThaiHoatDong ? "kích hoạt" : "vô hiệu hóa";
                TempData["SuccessMessage"] = $"Đã {status} tài khoản {user.HoTen} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng!";
            }

            return RedirectToAction(nameof(ManageUsers));
        }
    }
}
