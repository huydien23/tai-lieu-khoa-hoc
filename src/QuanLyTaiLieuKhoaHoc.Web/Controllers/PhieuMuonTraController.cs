using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Services;

namespace QuanLyTaiLieuKhoaHoc.Web.Controllers
{
    [Authorize]
    public class PhieuMuonTraController : Controller
    {
        private readonly IPhieuMuonTraService _phieuService;
        private readonly UserManager<NguoiDung> _userManager;

        public PhieuMuonTraController(IPhieuMuonTraService phieuService, UserManager<NguoiDung> userManager)
        {
            _phieuService = phieuService;
            _userManager = userManager;
        }

        // Chỉ thủ thư mới được thao tác mượn tài liệu
        [HttpPost]
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> GuiYeuCauMuon(int maTaiLieu, string? ghiChu)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _phieuService.GuiYeuCauMuonAsync(maTaiLieu, user.Id, ghiChu);
            if (result)
                return Ok(new { success = true, message = "Gửi yêu cầu mượn thành công!" });
            return BadRequest(new { success = false, message = "Gửi yêu cầu thất bại!" });
        }

        // Thủ thư xem danh sách yêu cầu chờ duyệt
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> DanhSachYeuCauChoDuyet()
        {
            var list = await _phieuService.LayDanhSachYeuCauChoDuyetAsync();
            return View(list);
        }

        // Thủ thư duyệt yêu cầu mượn
        [Authorize(Roles = "ThuThu")]
        [HttpPost]
        public async Task<IActionResult> DuyetYeuCauMuon(int maPhieu)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _phieuService.DuyetYeuCauMuonAsync(maPhieu, user.Id);
            if (result)
                return Ok(new { success = true, message = "Duyệt yêu cầu thành công!" });
            return BadRequest(new { success = false, message = "Duyệt yêu cầu thất bại!" });
        }

        // Thủ thư từ chối yêu cầu mượn
        [Authorize(Roles = "ThuThu")]
        [HttpPost]
        public async Task<IActionResult> TuChoiYeuCauMuon(int maPhieu, string? lyDo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _phieuService.TuChoiYeuCauMuonAsync(maPhieu, user.Id, lyDo);
            if (result)
                return Ok(new { success = true, message = "Từ chối yêu cầu thành công!" });
            return BadRequest(new { success = false, message = "Từ chối yêu cầu thất bại!" });
        }

        // Thủ thư xác nhận trả tài liệu
        [Authorize(Roles = "ThuThu")]
        [HttpPost]
        public async Task<IActionResult> XacNhanTraTaiLieu(int maPhieu)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _phieuService.XacNhanTraTaiLieuAsync(maPhieu, user.Id);
            if (result)
                return Ok(new { success = true, message = "Xác nhận trả tài liệu thành công!" });
            return BadRequest(new { success = false, message = "Xác nhận trả tài liệu thất bại!" });
        }

        // Thủ thư xác nhận trả tài liệu với ngày trả và ghi chú
        [Authorize(Roles = "ThuThu")]
        [HttpPost]
        public async Task<IActionResult> TraTaiLieu(int MaPhieu, DateTime NgayTra, string? GhiChu, string tinhTrang)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _phieuService.TraTaiLieuAsync(MaPhieu, user.Id, NgayTra, GhiChu, tinhTrang);
            if (result)
                return Json(new { success = true, message = "Trả tài liệu thành công!" });
            return Json(new { success = false, message = "Trả tài liệu thất bại!" });
        }

        // Lịch sử mượn/trả của người dùng
        public async Task<IActionResult> LichSuMuonTra()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var list = await _phieuService.LayLichSuMuonTraCuaNguoiDungAsync(user.Id);
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> LapPhieuMuon(int maPhieu)
        {
            var phieu = await _phieuService.LayPhieuMuonTraByIdAsync(maPhieu);
            if (phieu == null) return NotFound();
            var sinhVien = phieu.NguoiMuon;
            var taiLieu = phieu.TaiLieu;
            var vm = new QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels.LapPhieuMuonViewModel
            {
                MaPhieu = phieu.MaPhieu,
                HoTen = sinhVien?.HoTen ?? "",
                MSSV = sinhVien?.MaSo ?? "",
                Email = sinhVien?.Email ?? "",
                ChuyenNganh = sinhVien?.ChuyenNganh?.TenChuyenNganh ?? "",
                TenTaiLieu = taiLieu?.TenTaiLieu ?? "",
                TacGia = taiLieu?.TacGia ?? "",
                NgayMuon = phieu.NgayMuon,
                LyDo = phieu.GhiChu ?? "",
                NgayTra = phieu.NgayTra,
                NgayTraDuKien = phieu.NgayTraDuKien,
                IsFromRequest = true
            };
            return PartialView("~/Views/Shared/_LapPhieuMuonModal.cshtml", vm);
        }

        [HttpPost]
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> LapPhieuMuon(int maPhieu, DateTime ngayMuon, DateTime ngayTraDuKien)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _phieuService.LapPhieuMuonAsync(maPhieu, user.Id, ngayMuon, ngayTraDuKien);
            if (result)
                return Json(new { success = true, message = "Lập phiếu mượn thành công!" });
            return Json(new { success = false, message = "Lập phiếu mượn thất bại!" });
        }

        [HttpGet]
        public async Task<IActionResult> XemPhieu(int maPhieu)
        {
            var phieu = await _phieuService.LayPhieuMuonTraByIdAsync(maPhieu);
            if (phieu == null) return NotFound();
            return PartialView("~/Views/PhieuMuonTra/_ChiTietPhieuModal.cshtml", phieu);
        }

        [HttpGet]
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> LapPhieuTra(int maPhieu)
        {
            var phieu = await _phieuService.LayPhieuMuonTraByIdAsync(maPhieu);
            if (phieu == null) return NotFound();
            var sinhVien = phieu.NguoiMuon;
            var taiLieu = phieu.TaiLieu;
            var vm = new QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels.LapPhieuMuonViewModel
            {
                MaPhieu = phieu.MaPhieu,
                HoTen = sinhVien?.HoTen ?? "",
                MSSV = sinhVien?.MaSo ?? "",
                Email = sinhVien?.Email ?? "",
                ChuyenNganh = sinhVien?.ChuyenNganh?.TenChuyenNganh ?? "",
                TenTaiLieu = taiLieu?.TenTaiLieu ?? "",
                TacGia = taiLieu?.TacGia ?? "",
                NgayMuon = phieu.NgayMuon
            };
            return PartialView("~/Views/PhieuMuonTra/_LapPhieuTraModal.cshtml", vm);
        }

        [HttpPost]
        [Authorize(Roles = "ThuThu")]
        public async Task<IActionResult> LapPhieuTra(int maPhieu, DateTime ngayTra, string tinhTrang, string? ghiChu)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var result = await _phieuService.TraTaiLieuAsync(maPhieu, user.Id, ngayTra, ghiChu, tinhTrang);
            if (result)
                return Json(new { success = true, message = "Trả tài liệu thành công!" });
            return Json(new { success = false, message = "Trả tài liệu thất bại!" });
        }
    }
}
