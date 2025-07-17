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
            var danhSachYeuThich = await _context.YeuThichTaiLieu
                .Include(y => y.TaiLieu)
                .Where(y => y.UserId == userId && y.TaiLieu != null)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TaiLieuYeuThich = danhSachYeuThich
            };

            return View(model);
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
