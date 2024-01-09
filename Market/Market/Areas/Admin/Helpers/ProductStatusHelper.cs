using Microsoft.AspNetCore.Mvc.Rendering;

namespace Market.Areas.Admin.Helpers
{
    public static class ProductStatusHelper
    {
        public static List<SelectListItem> GetStatusList()
        {
            List<SelectListItem> listStatus = new List<SelectListItem>
        {
            new SelectListItem { Text = "Đang kinh doanh", Value = "1" },
            new SelectListItem { Text = "Ngừng kinh doanh", Value = "2" },
            // Thêm các trạng thái sản phẩm khác nếu cần
        };

            return listStatus;
        }
    }
}
