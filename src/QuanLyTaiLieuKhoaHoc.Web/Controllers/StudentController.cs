using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class StudentController : Controller
    {
        [ActionName("Dashboard-Student")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard Sinh viên";
            ViewData["UserRole"] = "student";
            var model = new QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels.DashboardViewModel();
            // TODO: Gán dữ liệu cho model nếu cần
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
