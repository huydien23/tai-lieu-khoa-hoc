using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class LecturerController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard Giảng viên";
            ViewData["UserRole"] = "lecturer";
            return View();
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
