using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class ThongKeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Thống kê & Báo cáo";
            return View();
        }

        public IActionResult Reports()
        {
            ViewData["Title"] = "Báo cáo Chi tiết";
            return View();
        }

        public IActionResult Charts()
        {
            ViewData["Title"] = "Biểu đồ Thống kê";
            return View();
        }

        [HttpGet]
        public IActionResult DocumentStats()
        {
            var stats = new
            {
                totalDocuments = 1234,
                totalBorrows = 567,
                totalUsers = 89,
                popularCategories = new[]
                {
                    new { name = "Khoa học máy tính", count = 45 },
                    new { name = "Toán học", count = 32 },
                    new { name = "Vật lý", count = 28 }
                }
            };

            return Json(stats);
        }

        [HttpGet]
        public IActionResult BorrowStats(string period = "month")
        {
            var data = period switch
            {
                "week" => new[] { 12, 19, 15, 25, 22, 18, 30 },
                "month" => new[] { 65, 59, 80, 81, 56, 55, 40, 45, 60, 70, 75, 85 },
                "year" => new[] { 450, 520, 480, 600, 590, 650, 720, 680, 750, 800, 820, 900 },
                _ => new[] { 65, 59, 80, 81, 56, 55, 40, 45, 60, 70, 75, 85 }
            };

            return Json(new { data, period });
        }

        [HttpGet]
        public IActionResult ExportReport(string type, string format = "excel")
        {
            TempData["SuccessMessage"] = $"Đang xuất báo cáo {type} định dạng {format}...";
            return RedirectToAction("Reports");
        }
    }
}
