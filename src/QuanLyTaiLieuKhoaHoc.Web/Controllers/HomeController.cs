using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;
using QuanLyTaiLieuKhoaHoc.Web.Services;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaiLieuService _taiLieuService;
        private readonly ApplicationDbContext _context;

        public HomeController(ITaiLieuService taiLieuService, ApplicationDbContext context)
        {
            _taiLieuService = taiLieuService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TongSoTaiLieu = await _context.TaiLieu.CountAsync(),
                TongSoNguoiDung = await _context.Users.CountAsync(),
                TongSoLuotTai = await _context.TaiLieu.SumAsync(t => t.LuotTai),
                TaiLieuMoiTrongThang = await _context.TaiLieu
                    .CountAsync(t => t.NgayTaiLen.Month == DateTime.Now.Month
                                && t.NgayTaiLen.Year == DateTime.Now.Year),

                TaiLieuMoiNhat = await _taiLieuService.GetTaiLieuMoiNhatAsync(6),
                TaiLieuPhoBien = await _taiLieuService.GetTaiLieuPhoBienAsync(6),

                ThongKeTheoChuyenNganh = await _context.TaiLieu
                    .Include(t => t.ChuyenNganh)
                    .GroupBy(t => t.ChuyenNganh!.TenChuyenNganh)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),

                ThongKeTheoLoaiTaiLieu = await _context.TaiLieu
                    .Include(t => t.LoaiTaiLieu)
                    .GroupBy(t => t.LoaiTaiLieu!.TenLoaiTaiLieu)
                    .ToDictionaryAsync(g => g.Key, g => g.Count())
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}