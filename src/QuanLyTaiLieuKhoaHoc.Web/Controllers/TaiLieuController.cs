using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class TaiLieuController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Danh sách Tài liệu";
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewData["Title"] = "Chi tiết Tài liệu";
            ViewBag.DocumentId = id;
            return View();
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Tài liệu mới";
            return View();
        }

        [HttpPost]
        public IActionResult Create(string title, string author, string category, string type)
        {
            // TODO: Implement create logic
            TempData["SuccessMessage"] = "Thêm tài liệu thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Chỉnh sửa Tài liệu";
            ViewBag.DocumentId = id;
            return View();
        }

        [HttpPost]
        public IActionResult Edit(int id, string title, string author, string category, string type)
        {
            // TODO: Implement edit logic
            TempData["SuccessMessage"] = "Cập nhật tài liệu thành công!";
            return RedirectToAction("Details", new { id });
        }

        public IActionResult Delete(int id)
        {
            ViewData["Title"] = "Xóa Tài liệu";
            ViewBag.DocumentId = id;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            // TODO: Implement delete logic
            TempData["SuccessMessage"] = "Xóa tài liệu thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search(string query, string category, string author)
        {
            ViewData["Title"] = "Tìm kiếm Tài liệu";
            ViewBag.Query = query;
            ViewBag.Category = category;
            ViewBag.Author = author;
            return View();
        }
    }
}
