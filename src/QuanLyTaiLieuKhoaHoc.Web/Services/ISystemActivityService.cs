using QuanLyTaiLieuKhoaHoc.Web.Models.ViewModels;

namespace QuanLyTaiLieuKhoaHoc.Web.Services
{
    public interface ISystemActivityService
    {
        Task<List<SystemActivity>> GetRecentActivitiesAsync(int count = 10);
        Task<List<SystemActivity>> GetActivitiesByTypeAsync(ActivityType activityType, int count = 10);
        Task<List<SystemActivity>> GetUnprocessedActivitiesAsync();
        Task<SystemActivity> CreateActivityAsync(SystemActivity activity);
        Task<bool> MarkActivityAsProcessedAsync(string activityId);
        Task<List<SystemActivity>> GetActivitiesForDashboardAsync();
    }
} 