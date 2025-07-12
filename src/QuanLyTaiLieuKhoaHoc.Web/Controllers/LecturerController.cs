using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class LecturerController : Controller
    {
        [ActionName("Dashboard-Lecturer")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard Giảng viên";
            ViewData["UserRole"] = "lecturer";
            var model = new QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels.DashboardViewModel();
            // TODO: Gán dữ liệu cho model nếu cần
            return View(model);
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
