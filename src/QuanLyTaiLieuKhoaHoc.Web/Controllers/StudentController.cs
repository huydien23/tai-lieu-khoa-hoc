using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard Sinh viên";
            ViewData["UserRole"] = "student";
            return View();
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
