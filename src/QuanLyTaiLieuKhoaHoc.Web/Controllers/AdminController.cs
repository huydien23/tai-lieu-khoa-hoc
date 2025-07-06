using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITaiLieuService _taiLieuService;

        public AdminController(ApplicationDbContext context, ITaiLieuService taiLieuService)
        {
            _context = context;
            _taiLieuService = taiLieuService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                TongSoTaiLieu = await _context.TaiLieu.CountAsync(),
                TongSoNguoiDung = await _context.Users.CountAsync(),
                TongSoLuotTai = await _context.TaiLieu.SumAsync(t => t.LuotTai),
                TaiLieuMoiTrongThang = await _context.TaiLieu
                    .CountAsync(t => t.NgayTaiLen.Month == DateTime.Now.Month
                                && t.NgayTaiLen.Year == DateTime.Now.Year),

                TaiLieuMoiNhat = await _taiLieuService.GetTaiLieuMoiNhatAsync(5),
                TaiLieuPhoBien = await _taiLieuService.GetTaiLieuPhoBienAsync(5),

                ThongKeTheoChuyenNganh = await _context.TaiLieu
                    .Include(t => t.ChuyenNganh)
                    .GroupBy(t => t.ChuyenNganh!.TenChuyenNganh)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),

                ThongKeTheoLoaiTaiLieu = await _context.TaiLieu
                    .Include(t => t.LoaiTaiLieu)
                    .GroupBy(t => t.LoaiTaiLieu!.TenLoaiTaiLieu)
                    .ToDictionaryAsync(g => g.Key, g => g.Count())
            };

            ViewData["Title"] = "Dashboard Quản trị";
            return View(model);
        }

        public IActionResult QuanLyTaiLieu()
        {
            ViewData["Title"] = "Quản lý Tài liệu";
            return View();
        }

        public IActionResult QuanLyNguoiDung()
        {
            ViewData["Title"] = "Quản lý Người dùng";
            return View();
        }

        public IActionResult BaoCaoThongKe()
        {
            ViewData["Title"] = "Báo cáo & Thống kê";
            return View();
        }
    }
}
