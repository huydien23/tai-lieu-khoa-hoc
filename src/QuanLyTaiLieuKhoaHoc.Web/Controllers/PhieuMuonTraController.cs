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

        // Sinh viên/giảng viên gửi yêu cầu mượn
        [HttpPost]
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

        // Lịch sử mượn/trả của người dùng
        public async Task<IActionResult> LichSuMuonTra()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var list = await _phieuService.LayLichSuMuonTraCuaNguoiDungAsync(user.Id);
            return View(list);
        }
    }
}
