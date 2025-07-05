using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class TimKiemController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Tìm kiếm Tài liệu";
            return View();
        }

        public IActionResult Advanced()
        {
            ViewData["Title"] = "Tìm kiếm Nâng cao";
            return View();
        }

        [HttpGet]
        public IActionResult Results(string query, string type, string category, string author, int? year)
        {
            ViewData["Title"] = "Kết quả Tìm kiếm";
            ViewBag.Query = query;
            ViewBag.Type = type;
            ViewBag.Category = category;
            ViewBag.Author = author;
            ViewBag.Year = year;
            return View();
        }

        [HttpPost]
        public IActionResult Search(string query, string searchType = "basic")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập từ khóa tìm kiếm";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Results", new { query });
        }

        [HttpGet]
        public IActionResult Suggestions(string term)
        {
            // TODO: Return JSON suggestions based on term
            var suggestions = new[]
            {
                "Khoa học máy tính",
                "Toán học ứng dụng",
                "Vật lý lý thuyết",
                "Hóa học hữu cơ",
                "Sinh học phân tử"
            }.Where(s => s.Contains(term, StringComparison.OrdinalIgnoreCase))
             .Take(5);

            return Json(suggestions);
        }
    }
}
