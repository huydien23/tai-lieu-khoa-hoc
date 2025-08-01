using Microsoft.EntityFrameworkCore;
using QuanLyTaiLieuKhoaHoc.Web.Data;
using QuanLyTaiLieuKhoaHoc.Web.Models;
using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public class SystemActivityService : ISystemActivityService
    {
        private readonly ApplicationDbContext _context;

        public SystemActivityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SystemActivity>> GetRecentActivitiesAsync(int count = 10)
        {
            return await GetActivitiesForDashboardAsync();
        }

        public async Task<List<SystemActivity>> GetActivitiesByTypeAsync(ActivityType activityType, int count = 10)
        {
            var activities = await GetActivitiesForDashboardAsync();
            return activities.Where(a => a.LoạiHoạtĐộng == activityType).Take(count).ToList();
        }

        public async Task<List<SystemActivity>> GetUnprocessedActivitiesAsync()
        {
            var activities = await GetActivitiesForDashboardAsync();
            return activities.Where(a => !a.ĐãXửLý).ToList();
        }

        public async Task<SystemActivity> CreateActivityAsync(SystemActivity activity)
        {
            return activity;
        }

        public async Task<bool> MarkActivityAsProcessedAsync(string activityId)
        {
            return true;
        }

        public async Task<List<SystemActivity>> GetActivitiesForDashboardAsync()
        {
            var activities = new List<SystemActivity>();

            // 1. Người dùng mới đăng ký (trong 24 giờ qua)
            var newUsers = await _context.Users
                .Where(u => u.NgayTao >= DateTime.Now.AddDays(-1))
                .OrderByDescending(u => u.NgayTao)
                .Take(3)
                .ToListAsync();

            foreach (var user in newUsers)
            {
                activities.Add(new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.NgườiDùngMớiĐăngKý,
                    TiêuĐề = "Người dùng mới đăng ký",
                    MôTả = $"{user.HoTen} ({user.VaiTro}) đã đăng ký tài khoản mới",
                    ThờiGian = user.NgayTao,
                    MứcĐộƯuTiên = ActivityPriority.Thấp,
                    NgườiThựcHiện = user.HoTen,
                    LiênKết = $"/Librarian/ManageUsers"
                });
            }

            // 2. Yêu cầu mượn tài liệu chờ duyệt
            var pendingRequests = await _context.PhieuMuonTra
                .Include(p => p.NguoiMuon)
                .Include(p => p.TaiLieu)
                .Where(p => p.TrangThai == TrangThaiPhieu.ChoDuyet)
                .OrderByDescending(p => p.NgayMuon)
                .Take(5)
                .ToListAsync();

            foreach (var request in pendingRequests)
            {
                activities.Add(new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.YêuCầuMượnTàiLiệu,
                    TiêuĐề = "Yêu cầu mượn tài liệu",
                    MôTả = $"{request.NguoiMuon?.HoTen} yêu cầu mượn \"{request.TaiLieu?.TenTaiLieu}\"",
                    ThờiGian = request.NgayMuon,
                    MứcĐộƯuTiên = ActivityPriority.TrungBình,
                    NgườiThựcHiện = request.NguoiMuon?.HoTen,
                    DữLiệuBổSung = request.MaPhieu.ToString(),
                    LiênKết = $"/Librarian/ManageBorrowReturn"
                });
            }

            // 3. Tài liệu sắp quá hạn (trong 3 ngày tới)
            var upcomingOverdue = await _context.PhieuMuonTra
                .Include(p => p.NguoiMuon)
                .Include(p => p.TaiLieu)
                .Where(p => p.TrangThai == TrangThaiPhieu.DaDuyet 
                           && p.NgayTra == null 
                           && p.NgayTraDuKien <= DateTime.Now.AddDays(3)
                           && p.NgayTraDuKien > DateTime.Now)
                .OrderBy(p => p.NgayTraDuKien)
                .Take(3)
                .ToListAsync();

            if (upcomingOverdue.Any())
            {
                activities.Add(new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.CảnhBáoQuáHạn,
                    TiêuĐề = "Cảnh báo quá hạn",
                    MôTả = $"{upcomingOverdue.Count} tài liệu sắp quá hạn trong 3 ngày tới",
                    ThờiGian = DateTime.Now,
                    MứcĐộƯuTiên = ActivityPriority.TrungBình,
                    DữLiệuBổSung = upcomingOverdue.Count.ToString(),
                    LiênKết = $"/Librarian/ManageBorrowReturn"
                });
            }

            // 4. Tài liệu đã quá hạn
            var overdueDocuments = await _context.PhieuMuonTra
                .Include(p => p.NguoiMuon)
                .Include(p => p.TaiLieu)
                .Where(p => p.TrangThai == TrangThaiPhieu.DaDuyet 
                           && p.NgayTra == null 
                           && p.NgayTraDuKien < DateTime.Now)
                .OrderBy(p => p.NgayTraDuKien)
                .Take(3)
                .ToListAsync();

            if (overdueDocuments.Any())
            {
                activities.Add(new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.TàiLiệuQuáHạn,
                    TiêuĐề = "Tài liệu quá hạn",
                    MôTả = $"{overdueDocuments.Count} tài liệu đã quá hạn cần xử lý ngay",
                    ThờiGian = DateTime.Now,
                    MứcĐộƯuTiên = ActivityPriority.Cao,
                    DữLiệuBổSung = overdueDocuments.Count.ToString(),
                    LiênKết = $"/Librarian/ManageBorrowReturn"
                });
            }

            // 5. Tài liệu mới được thêm (trong 24 giờ qua)
            var newDocuments = await _context.TaiLieu
                .Where(t => t.NgayTaiLen >= DateTime.Now.AddDays(-1))
                .OrderByDescending(t => t.NgayTaiLen)
                .Take(3)
                .ToListAsync();

            foreach (var document in newDocuments)
            {
                activities.Add(new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.TàiLiệuMớiĐượcThêm,
                    TiêuĐề = "Tài liệu mới được thêm",
                    MôTả = $"\"{document.TenTaiLieu}\" đã được thêm vào hệ thống",
                    ThờiGian = document.NgayTaiLen,
                    MứcĐộƯuTiên = ActivityPriority.Thấp,
                    DữLiệuBổSung = document.MaTaiLieu.ToString(),
                    LiênKết = $"/Librarian/ManageDocuments"
                });
            }

            // 6. Thống kê hoạt động hôm nay
            var todayBorrows = await _context.PhieuMuonTra
                .Where(p => p.NgayMuon.Date == DateTime.Now.Date && p.TrangThai == TrangThaiPhieu.DaDuyet)
                .CountAsync();

            if (todayBorrows > 0)
            {
                activities.Add(new SystemActivity
                {
                    LoạiHoạtĐộng = ActivityType.ThốngKêHoạtĐộng,
                    TiêuĐề = "Thống kê hoạt động",
                    MôTả = $"Hôm nay có {todayBorrows} lượt mượn tài liệu mới",
                    ThờiGian = DateTime.Now,
                    MứcĐộƯuTiên = ActivityPriority.Thấp,
                    DữLiệuBổSung = todayBorrows.ToString(),
                    LiênKết = $"/Librarian/Statistics"
                });
            }

            // Sắp xếp theo thời gian và mức độ ưu tiên
            return activities
                .OrderByDescending(a => a.MứcĐộƯuTiên)
                .ThenByDescending(a => a.ThờiGian)
                .Take(10)
                .ToList();
        }
    }
} 