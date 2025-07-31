namespace QuanLyTaiLieuKhoaHoc.Web.Models
{
    public class BreadcrumbItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        
        public BreadcrumbItem(string text, string url = null, string icon = null)
        {
            Text = text;
            Url = url;
            Icon = icon ?? "fas fa-angle-right";
        }
    }
} 