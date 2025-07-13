using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    public class TimKiemController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index(string? q = null, int? chuyenNganh = null, int? loaiTaiLieu = null, int trang = 1)
        {
            // Redirect to TaiLieu/Index with parameters preserved
            return RedirectToAction("Index", "TaiLieu", new 
            { 
                timKiem = q, 
                maChuyenNganh = chuyenNganh, 
                maLoaiTaiLieu = loaiTaiLieu, 
                trang = trang 
            });
        }

        [HttpGet]
        public IActionResult Suggestions(string term)
        {
            // Redirect to TaiLieu/Suggestions
            return RedirectToAction("Suggestions", "TaiLieu", new { term = term });
        }
    }
}
