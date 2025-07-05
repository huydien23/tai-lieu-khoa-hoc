using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard Thủ thư";
            ViewData["UserRole"] = "admin";
            return View();
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
