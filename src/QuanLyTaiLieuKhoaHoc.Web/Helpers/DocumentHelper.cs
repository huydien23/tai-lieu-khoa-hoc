namespace QuanLyTaiLieuKhoaHoc.Web.Helpers
{
    public static class DocumentHelper
    {
        public static string GetDocumentTypeClass(string tenLoaiTaiLieu)
        {
            if (string.IsNullOrEmpty(tenLoaiTaiLieu))
                return "type-default";

            return tenLoaiTaiLieu.ToLower() switch
            {
                "bài báo khoa học" => "type-scientific",
                "sách giáo khoa" => "type-textbook",
                "tài liệu tham khảo" => "type-reference",
                "luận văn" => "type-thesis",
                "báo cáo" => "type-report",
                _ => "type-default"
            };
        }
    }
} 